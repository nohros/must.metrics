using System.Collections.Generic;

namespace Nohros.Metrics
{
  /// <summary>
  /// Regsitry to keep track of <see cref="IMetric"/> objects.
  /// </summary>
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
