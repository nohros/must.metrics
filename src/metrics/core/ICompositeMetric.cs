using System.Collections.Generic;

namespace Nohros.Metrics
{
  /// <summary>
  /// Defines a mixin for metrics that are composed of a number of sub-metrics.
  /// </summary>
  /// <seealso cref="StatsTimer"/>
  /// <seealso cref="Histogram"/>
  public interface ICompositeMetric : IMetric, IEnumerable<IMetric>
  {
    /// <summary>
    /// Gets the list of sub-metrics for this composite.
    /// </summary>
    ICollection<IMetric> Metrics { get; }
  }
}
