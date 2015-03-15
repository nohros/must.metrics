using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// Method that is called when a new metric is added to a registry.
  /// </summary>
  /// <param name="config">
  /// The id associated with the added metric.
  /// </param>
  /// <param name="metric">
  /// The added metric.
  /// </param>
  public delegate void MetricAddedEventHandler(MetricConfig config, IMetric metric);
}
