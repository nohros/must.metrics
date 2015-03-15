using System;

namespace Nohros.Metrics.Data
{
  public interface IMetricsDao
  {
    /// <summary>
    /// Store a metric in the repository.
    /// </summary>
    void Persist(MetricDto metric);
  }
}
