using System;

namespace Nohros.Metrics.Annotations
{
  /// <summary>
  /// Represents a list of tags to associate with all metrics in an instance.
  /// </summary>
  /// <remarks>
  /// The tags will be queried when the instance is registered with the
  /// <see cref="AppMetrics.RegisterObject(object)"/> and used to provide a
  /// common base tags for all fields of type <see cref="IMetric"/>. Tags
  /// provided on the fields will override these tags if there is a common key.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field,
    AllowMultiple = false)]
  public class TagsAttribute : Attribute
  {
  }
}
