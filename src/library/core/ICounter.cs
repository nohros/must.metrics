using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A metric for tracking how often some event is occuring.
  /// </summary>
  public interface ICounter
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

    /// <summary>
    /// Decrements the counter by one.
    /// </summary>
    void Decrement();

    /// <summary>
    /// Decrements the counter by <paramref name="n"/>
    /// </summary>
    /// <param name="n">
    /// The amount by which the counter will be increased.
    /// </param>
    void Decrement(long n);
  }
}
