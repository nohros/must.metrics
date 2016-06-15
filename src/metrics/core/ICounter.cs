using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A metric for tracking how often some event is occuring.
  /// </summary>
  /// <remarks>
  /// The <see cref="Counter"/> is a numeric value that get incremented when
  /// some event such a client connecting to a server, occurs.
  /// <para>
  /// When reported to monitoring system, a counter will typically be converted
  /// to a rate of change by comparing two samples.
  /// </para>
  /// <para>
  /// <see cref="Counter"/> values  should be monotonically increasing.
  /// </para>
  /// </remarks>
  public interface ICounter : IMetric
  {
    /// <summary>
    /// Increments the counter by one.
    /// </summary>
    void Increment();

    /// <summary>
    /// Increments the counter by <paramref name="n"/>.
    /// </summary>
    /// <param name="n">
    /// The amount by which the counter will be increased.
    /// </param>
    void Increment(long n);
  }
}
