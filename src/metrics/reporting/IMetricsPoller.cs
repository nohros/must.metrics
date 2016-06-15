using System;

namespace Nohros.Metrics.Reporting
{
  /// <summary>
  /// A poller that can be used to fetch current values from a set
  /// of metrics on demand.
  /// </summary>
  public interface IMetricsPoller
  {
    /// <summary>
    /// Fetch the current values of a set of metrics.
    /// </summary>
    void Poll();

    /// <summary>
    /// Fetch the current values of a set of metrics that match the provided
    /// <paramref name="predicate"/>.
    /// </summary>
    /// <param name="predicate">
    /// A <see cref="Func{TResult}"/> that restricts the measures that should
    /// be fetched.
    /// </param>
    void Poll(Func<MetricConfig, bool> predicate);
  }
}
