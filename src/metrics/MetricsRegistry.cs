using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Nohros.Metrics
{
  /// <summary>
  /// A basic implementation of <see cref="IMetricsRegistry"/> class backed
  /// by a <see cref="HashSet{T}"/>.
  /// </summary>
  public class MetricsRegistry : IMetricsRegistry
  {
    readonly HashSet<IMetric> metrics_;
    ReadOnlyCollection<IMetric> readonly_metrics_;
    bool changed_;

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricsRegistry"/> class.
    /// </summary>
    public MetricsRegistry() {
      metrics_ = new HashSet<IMetric>();
      changed_ = false;
    }

    /// <inheritdoc/>
    public void Register(IMetric metric) {
      changed_ = true;
      metrics_.Add(metric);
    }

    /// <inheritdoc/>
    public void Unregister(IMetric metric) {
      changed_ = true;
      metrics_.Remove(metric);
    }

    /// <inheritdoc/>
    public ICollection<IMetric> Metrics {
      get {
        if (changed_) {
          readonly_metrics_ = new ReadOnlyCollection<IMetric>(metrics_.ToList());
        }
        return readonly_metrics_;
      }
    }
  }
}
