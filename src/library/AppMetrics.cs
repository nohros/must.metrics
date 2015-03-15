using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Nohros.Extensions;
using Nohros.Metrics.Annotations;

namespace Nohros.Metrics
{
  /// <summary>
  /// A default registry that delegates all actions to a either class
  /// configured  through the <see cref="Configure"/> method, the class
  /// defined by the the key "DefaultMetricsRegistry" in the
  /// <see cref="ConfigurationManager.AppSettings"/> configuration section,
  /// or the default registry(<see cref="MetricsRegistry"/>).
  /// <para>
  /// The class defined by the key "DefaultMetricsRegistry" must have a
  /// constructor with no arguments. If the class cannot be loaded
  /// de defualt metrics registry approach will be used.
  /// </para>
  /// </summary>
  public class AppMetrics
  {
    /// <summary>
    /// Wraps another <see cref="ICompositeMetric"/> object providing an
    /// alternative configuration.
    /// </summary>
    class CompositeMetricWrapper : AbstractMetric, ICompositeMetric
    {
      public CompositeMetricWrapper(Tags tags, ICompositeMetric composite)
        : base(composite.Config.WithAdditionalTags(tags)) {
        List<IMetric> wrapped =
          composite
            .Metrics
            .Select(x => Wrap(x, Config.Tags))
            .ToList();
        Metrics = new ReadOnlyCollection<IMetric>(wrapped);
      }

      /// <inheritdoc/>
      public IEnumerator<IMetric> GetEnumerator() {
        return Metrics.GetEnumerator();
      }

      /// <inheritdoc/>
      IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
      }

      /// <inheritdoc/>
      public ICollection<IMetric> Metrics { get; private set; }

      /// <inheritdoc/>
      protected internal override Measure Compute(long tick) {
        return CreateMeasure(Metrics.Count);
      }
    }

    /// <summary>
    /// A class that is used to configure the behavior of the
    /// global <see cref="AppMetrics"/>.
    /// </summary>
    public class Configurer
    {
      internal Configurer() {
        process_tags_
          .WithTag("ns.node", Environment.MachineName);
      }

      /// <summary>
      /// Define the collection of tags that should be associated with
      /// all metrics registered through the <see cref="AppMetrics"/>.
      /// </summary>
      /// <param name="tags">
      /// The list of tags to be associated with all metrics registered
      /// through the <see cref="AppMetrics"/>.
      /// </param>
      /// <remarks>
      /// This method should be used only if you want to remove all the
      /// tags that is automatically created by the metrics library and
      /// provide your own list of tags. If the intend is to increment
      /// the default list of tags, use the <see cref="WithAdditionalTags(Tags)"/>
      /// method instead.
      /// </remarks>
      public Configurer WithTags(Tags tags) {
        process_tags_ = new Tags.Builder(tags);
        return this;
      }

      /// <summary>
      /// Adds the given collection of tags to the current list of
      /// tags that is associated with all the metrics registered through
      /// the <see cref="AppMetrics"/>.
      /// </summary>
      /// <param name="tags">
      /// The list of tags to be associated with all metrics registered
      /// through the <see cref="AppMetrics"/>.
      /// </param>
      public Configurer WithAdditionalTags(Tags tags) {
        process_tags_.WithTags(tags);
        return this;
      }

      /// <summary>
      /// Adds the given collection of tags to the current list of
      /// tags that will be associated with all the metrics registered
      /// through the <see cref="AppMetrics"/>.
      /// </summary>
      /// <param name="tags">
      /// The list of tags to be merged with the list of all tags that
      /// will be associated with all metrics registered through the
      /// <see cref="AppMetrics"/>.
      /// </param>
      public Configurer WithAdditionalTags(IEnumerable<Tag> tags) {
        process_tags_.WithTags(tags);
        return this;
      }

      /// <summary>
      /// Adds the given tag to the current list of tags that will be
      /// associated with all the metrics registered through
      /// the <see cref="AppMetrics"/>.
      /// </summary>
      /// <param name="tag">
      /// The tag to be added to the list of tags that will be associated
      /// with all metrics registered through the <see cref="AppMetrics"/>.
      /// </param>
      public Configurer WithAdditionalTag(Tag tag) {
        process_tags_.WithTag(tag);
        return this;
      }

      /// <summary>
      /// Adds the given tag to the current list of tags that will be
      /// associated with all the metrics registered through
      /// the <see cref="AppMetrics"/>.
      /// </summary>
      /// <param name="name">
      /// The name of the tag to be added to the list of tags that
      /// will be associated with all metrics registered through the
      /// <see cref="AppMetrics"/>.
      /// </param>
      /// <param name="value">
      /// The value of the tag to be added to the list of tags that
      /// will be associated with all metrics registered through the
      /// <see cref="AppMetrics"/>.
      /// </param>
      public Configurer WithAdditionalTag(string name, string value) {
        process_tags_.WithTag(name, value);
        return this;
      }
    }

    /// <summary>
    /// Wraps another <see cref="IMetric"/> object providing an alternative
    /// configuration.
    /// </summary>
    class MetricWrapper : IMetric
    {
      readonly IMetric metric_;

      /// <summary>
      /// Initializes a new instance of the <see cref="MetricWrapper"/> class
      /// by using the given <paramref name="tags"/> and
      /// <paramref name="metric"/>.
      /// </summary>
      /// <param name="tags">
      /// The alternate configuration.
      /// </param>
      /// <param name="metric">
      /// The metric to be wrapped.
      /// </param>
      public MetricWrapper(Tags tags, IMetric metric) {
        Config = metric.Config.WithAdditionalTags(tags);
        metric_ = metric;
      }

      /// <inheritdoc/>
      public void GetMeasure(Action<Measure> callback) {
        metric_.GetMeasure(m => callback(WrapMeasure(m)));
      }

      /// <inheritdoc/>
      public void GetMeasure<T>(Action<Measure, T> callback, T state) {
        metric_.GetMeasure(m => callback(WrapMeasure(m), (state)));
      }

      /// <inheritdoc/>
      public MetricConfig Config { get; private set; }

      Measure WrapMeasure(Measure measure) {
        return new Measure(Config, measure.Value, measure.IsObservable);
      }
    }

    const string kDefaultMetricsRegistryKey = "DefaultMetricsRegistry";

    static bool configured_;
    static Tags.Builder process_tags_;

    static AppMetrics() {
      string default_metrics_registry_class =
        ConfigurationManager.AppSettings[kDefaultMetricsRegistryKey];

      if (default_metrics_registry_class != null) {
        var runtime_type = new RuntimeType(default_metrics_registry_class);

        Type type = RuntimeType.GetSystemType(runtime_type);

        if (type != null) {
          ForCurrentProcess =
            RuntimeTypeFactory<IMetricsRegistry>
              .CreateInstanceFallback(runtime_type);
        }
      }

      if (ForCurrentProcess == null) {
        ForCurrentProcess = new MetricsRegistry();
      }

      process_tags_ = new Tags.Builder();

      Configure();

      // Mark the class as not configured to allow users to
      // overrite the default behavior.
      configured_ = false;
    }

    /// <summary>
    /// Configure the behavior of the <see cref="AppMetrics"/>.
    /// </summary>
    /// <returns>
    /// A <see cref="Configurer"/> that can be used to manage the
    /// behavior of the <see cref="AppMetrics"/>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="Configure"/> method was already called.
    /// </exception>
    /// <remarks>
    /// This method should be called before the usage of any
    /// method of the <see cref="AppMetrics"/>, usually at the application
    /// startup.
    ///<para>
    /// This method should be called once per application and calling it
    /// a second time will throw an exception.
    /// </para>
    /// </remarks>
    public static Configurer Configure() {
      if (configured_) {
        throw new ArgumentException("The AppMetrics was already configured.");
      }
      configured_ = true;
      return new Configurer();
    }

    /// <summary>
    /// Register the given metric in the default registry.
    /// </summary>
    public static void Register(IMetric metric) {
      ForCurrentProcess.Register(metric);
    }

    /// <summary>
    /// Register the given metrics in the default registry.
    /// </summary>
    public static void Register(IEnumerable<IMetric> metrics) {
      foreach (var metric in metrics) {
        Register(metric);
      }
    }

    /// <summary>
    /// Unregister the given metrics from the default registry.
    /// </summary>
    public static void Unregister(IMetric metric) {
      ForCurrentProcess.Unregister(metric);
    }

    /// <summary>
    /// Register a <see cref="ICompositeMetric"/> that is a composite for all
    /// metric fields and annotated attributes of a given object.
    /// </summary>
    /// <remarks>
    /// Object to search for metrics on. All fields of type
    /// <see cref="IMetric"/> and fields/methods with a
    /// <see cref="MetricAttribute"/> attribute will be extracted and
    /// returned using <see cref="ICompositeMetric.Metrics"/>
    /// <para>
    /// Note that the <see cref="RegisterObject(object)"/>  will use
    /// reflection to add all instances of <see cref="IMetric"/> that have
    /// been declared, and also add a tag with the value set to class simple
    /// name (<see cref="Type.Name"/>) and namespace (<see cref="Type.Namespace"/>).
    /// </para>
    /// </remarks>
    /// <returns>
    /// A <see cref="ICompositeMetric"/> based on the fields of the class of
    /// <paramref name="obj"/>.
    /// </returns>
    public static ICompositeMetric RegisterObject(object obj) {
      // The tags defined at the class level should be merged with the tags
      // defined for the fields and methods of the class.
      Type klass = obj.GetType();
      Tags tags =
        new Tags.Builder()
          .WithTags(process_tags_.Build())
          .WithTags(GetTags(klass, true)) // static class tags
          .WithTags(GetTagsList(obj)) // dynamic class tags
          .WithTag("class", klass.Name)
          .WithTag("namespace", klass.Namespace)
          .Build();

      var metrics = new List<IMetric>();
      for (Type type = klass; type != null; type = type.BaseType) {
        AddMetrics(metrics, tags, obj, type);
      }

      var config = new MetricConfig("annotated", tags);
      var composite = new BasicCompositeMetric(config, metrics);
      Register((IMetric) composite);

      return composite;
    }

    /// <summary>
    /// Extract all fields of <paramref name="obj"/> that are of type
    /// <see cref="IMetric"/> and add them to <paramref name="metrics"/>.
    /// </summary>
    /// <param name="metrics">
    /// A <see cref="List{T}"/> object to add the extracted metrics.
    /// </param>
    /// <param name="tags">
    /// A <see cref="Tags"/> object contained a list of tags that should be
    /// added to the extracted metrics.
    /// </param>
    /// <param name="obj">
    /// The object to extract the monitor fields.
    /// </param>
    /// <param name="type">
    /// The type to extract the fields. This type is one of the types of
    /// the <paramref name="obj"/> hierarchy.
    /// </param>
    static void AddMetrics(List<IMetric> metrics, Tags tags, object obj,
      Type type) {
      IEnumerable<FieldInfo> metric_fields =
        GetFields(type)
          .Where(IsMetricType);

      foreach (FieldInfo field in metric_fields) {
        var metric = field.GetValue(obj) as IMetric;
        if (metric == null) {
          throw new ArgumentNullException(Resources.NullAnnotatedField.Fmt());
        }
        metrics.Add(Wrap(metric, tags, field));
      }
    }

    static IMetric Wrap(IMetric metric, Tags tags, FieldInfo field) {
      var field_tags =
        new Tags.Builder(tags)
          .WithTags(tags)
          .WithTags(metric.Config.Tags)
          .WithTags(GetTags(field))
          .Build();
      return Wrap(metric, field_tags);
    }

    static IMetric Wrap(IMetric metric, Tags tags) {
      if (metric is ICompositeMetric) {
        return new CompositeMetricWrapper(tags, metric as ICompositeMetric);
      }
      return new MetricWrapper(tags, metric);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    static IEnumerable<Tag> GetTagsList(object obj) {
      FieldInfo field =
        GetFields(obj.GetType())
          .Where(IsTagList)
          .FirstOrDefault();

      if (field != null) {
        return (IEnumerable<Tag>) field.GetValue(obj);
      }

      MethodInfo method =
        GetMethods(obj.GetType())
          .Where(IsTagList)
          .FirstOrDefault();

      if (method != null) {
        return (IEnumerable<Tag>) method.Invoke(obj, new object[0]);
      }
      return Enumerable.Empty<Tag>();
    }

    /// <summary>
    /// Checks if the given <paramref name="field"/> represents a
    /// type that can be is assiged to an <see cref="IMetric"/>.
    /// </summary>
    /// <param name="field">
    /// A <see cref="FieldInfo"/> contianing the type information
    /// about the field to check.
    /// </param>
    /// <returns>
    /// <c>true</c> if the type associated with the given
    /// <paramref name="field"/> can be assigned to a
    /// <see cref="IMetric"/>.
    /// </returns>
    static bool IsMetricType(FieldInfo field) {
      return typeof (IMetric).IsAssignableFrom(field.FieldType);
    }

    /// <summary>
    /// Checks if the given <paramref name="field"/> represents a
    /// type that can be is assiged to an
    /// <see cref="IEnumerable{T}"/> of <see cref="Tag"/>.
    /// </summary>
    /// <param name="field">
    /// A <see cref="FieldInfo"/> contianing the type information
    /// about the field to check.
    /// </param>
    /// <returns>
    /// <c>true</c> if the type associated with the given
    /// <paramref name="field"/> can be assigned to a
    /// <see cref="IEnumerable{T}"/> of <see cref="Tag"/>.
    /// </returns>
    static bool IsTagList(FieldInfo field) {
      return IsTagList(field.FieldType);
    }

    /// <summary>
    /// Checks if the given <paramref name="method"/> returns am
    /// object that can be is assiged to an
    /// <see cref="IEnumerable{T}"/> of <see cref="Tag"/>.
    /// </summary>
    /// <param name="method">
    /// A <see cref="MethodInfo"/> contianing the type information
    /// about the method to check.
    /// </param>
    /// <returns>
    /// <c>true</c> if the return type of the given
    /// <paramref name="method"/> can be assigned to a
    /// <see cref="IEnumerable{T}"/> of <see cref="Tag"/>.
    /// </returns>
    static bool IsTagList(MethodInfo method) {
      return IsTagList(method.ReturnType);
    }

    /// <summary>
    /// Checks if the given type is assignable to an
    /// <see cref="IEnumerable{T}"/> of <see cref="Tag"/>
    /// </summary>
    /// <param name="type">
    /// The type to check for assignability.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="type"/> can be assigned
    /// to a <see cref="IEnumerable{T}"/> of <see cref="Tag"/>.
    /// </returns>
    static bool IsTagList(Type type) {
      return typeof (IEnumerable<Tag>).IsAssignableFrom(type);
    }

    static IEnumerable<FieldInfo> GetFields(Type type) {
      const BindingFlags kFlags =
        BindingFlags.Instance |
          BindingFlags.Static |
          BindingFlags.Public |
          BindingFlags.NonPublic |
          BindingFlags.DeclaredOnly;

      return type.GetFields(kFlags);
    }

    static IEnumerable<MethodInfo> GetMethods(Type type) {
      const BindingFlags kFlags =
        BindingFlags.Instance |
          BindingFlags.Static |
          BindingFlags.Public |
          BindingFlags.NonPublic |
          BindingFlags.DeclaredOnly;

      return type.GetMethods(kFlags);
    }

    /// <summary>
    /// Gets the tags declared for the given member.
    /// </summary>
    /// <param name="member">
    /// The type to get the tags.
    /// </param>
    /// <param name="inherit">
    /// <c>true</c> to search this member's inheritance chain to find the
    /// tags; otherwise, <c>false</c>. This parameter is ignored for
    /// fields.
    /// </param>
    /// <returns>
    /// A <see cref="Tags"/> object containing the tags declared for the
    /// given member.
    /// </returns>
    static IEnumerable<Tag> GetTags(MemberInfo member, bool inherit = false) {
      return
        GetAttributes<TagAttribute>(member, inherit)
          .Select(t => t.Tag);
    }

    static IEnumerable<T> GetAttributes<T>(MemberInfo member,
      bool inherit = false) {
      return
        member
          .GetCustomAttributes(typeof (T), inherit)
          .Cast<T>();
    }

    /// <summary>
    /// Gets the default configured <see cref="IMetricsRegistry"/>.
    /// </summary>
    public static IMetricsRegistry ForCurrentProcess { get; private set; }
  }
}
