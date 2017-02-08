using System;
using System.Collections.Generic;

namespace Nohros.Metrics
{
  /// <summary>
  /// A implementation of the <see cref="IMetricsRegistry"/> class that
  /// fowrads all its methods to another <see cref="IMetricsRegistry"/>.
  /// </summary>
  public class ForwardingMetricsRegistry : IMetricsRegistry
  {
    readonly IMetricsRegistry registry_;

    public ForwardingMetricsRegistry(IMetricsRegistry registry) {
      registry_ = registry;
    }

    /// <inheritdoc/>
    public ICollection<IMetric> Metrics {
      get { return registry_.Metrics; }
    }

    /// <inheritdoc/>
    public void Register(IMetric metric) {
      registry_.Register(metric);
    }

    /// <inheritdoc/>
    public void Unregister(IMetric metric) {
      registry_.Unregister(metric);
    }
  }
}
