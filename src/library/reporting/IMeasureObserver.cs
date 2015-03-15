using System;

namespace Nohros.Metrics.Reporting
{
  /// <summary>
  /// A reporter that receives updates about measures.
  /// </summary>
  public interface IMeasureObserver
  {
    /// <summary>
    /// Observe the most recent measure of a metric.
    /// </summary>
    /// <param name="measure">
    /// The most recent measure of a metric.
    /// </param>
    /// <param name="timestamp">
    /// The date and time when the measure was computed.
    /// </param>
    void Observe(Measure measure, DateTime timestamp);
  }
}
