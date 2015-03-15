using System.Collections.Generic;

namespace Nohros.Metrics
{
  public interface IMetricsRegistry
  {
    /// <summary>
    /// Gets a <see cref="ICollection{T}"/> containing all the registered
    /// metrics.
    /// </summary>
    ICollection<IMetric> Metrics { get; }

    /// <summary>
    /// Register the gieven metric in the registry.
    /// </summary>
    void Register(IMetric metric);

    /// <summary>
    /// Unregister the given metric from the registry.
    /// </summary>
    void Unregister(IMetric metric);
  }
}
