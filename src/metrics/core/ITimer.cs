using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A timer metric which aggregates timing durations and provides duration
  /// statistics, plus throughput statistics
  /// </summary>
  public interface ITimer : IMetric
  {
    /// <summary>
    /// Times and records the duration of event.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by
    /// <paramref name="method"/></typeparam>
    /// <param name="method">
    /// A method whose duration should be timed.
    /// </param>
    /// <returns>
    /// The value returned by <paramref name="method"/>.
    /// </returns>
    /// <exception cref="Exception">
    /// The exception throwed by <paramref name="method"/>.
    /// </exception>
    T Time<T>(Func<T> method);

    /// <summary>
    /// Times and records the duration of event.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by
    /// <paramref name="method"/></typeparam>
    /// <param name="method">A method whose duration should be timed.</param>
    /// <returns>The value returned by <paramref name="method"/>.</returns>
    /// <exception cref="Exception">Exception if <paramref name="method"/>
    /// throws an <see cref="Exception"/>.</exception>
    void Time(Action method);

    /// <summary>
    /// Gets a timing <see cref="TimerContext"/>, which measures an elapsed
    /// time in nanoseconds.
    /// </summary>
    /// <returns>
    /// A new <see cref="TimerContext"/>.
    /// </returns>
    TimerContext Time();

    /// <summary>
    /// Adds a recorded duration.
    /// </summary>
    /// <param name="duration">The length of the duration.</param>
    void Update(TimeSpan duration);
  }
}
