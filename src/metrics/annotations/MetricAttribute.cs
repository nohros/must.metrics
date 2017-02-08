using System;

namespace Nohros.Metrics.Annotations
{
  /// <summary>
  /// Defines a base class for the metric's attributes.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
  public abstract class MetricAttribute : Attribute
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MetricAttribute"/> by
    /// using the given metric <paramref name="name"/>.
    /// </summary>
    /// <param name="name">
    /// A string that should be used to identify a metric.
    /// </param>
    /// <param name="type">The metric's type</param>
    /// <seealso cref="MetricConfig"/>
    /// <seealso cref="MetricConfig.Name"/>
    protected MetricAttribute(string name, MetricType type) {
      Name = name;
      MetricType = type;
    }

    /// <summary>
    /// Gets the name of the metric.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the type of the metric.
    /// </summary>
    public MetricType MetricType { get; protected set; }
  }
}
