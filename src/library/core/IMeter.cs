using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A meter metric which measures mean throughput.
  /// </summary>
  public interface IMeter : IMetric
  {
    /// <summary>
    /// Mark the occurrence of an event.
    /// </summary>
    void Mark();

    /// <summary>
    /// Mark the occurrence of a given number of events.
    /// </summary>
    /// <param name="n">
    /// The number of events.
    /// </param>
    void Mark(long n);
  }
}
