using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// Defines a metric that is mapped to a particular step interval.
  /// </summary>
  public interface IStepMetric : IMetric
  {
    /// <summary>
    /// Gets the current measure for the given metric.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="Action"/> that should be called when the current measure
    /// for the current metric was computed.
    /// </param>
    /// <param name="reset">
    /// A flag that indicates if the metric's internal state should be reseted
    /// for the next step.
    /// </param>
    void GetMeasure(Action<Measure> callback, bool reset);

    /// <summary>
    /// Gets the current measure for the given metric.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="Action"/> that should be called when the current measure
    /// for the current metric was computed.
    /// </param>
    /// <param name="state">
    /// A value that will be passed to the callback when it is executed.
    /// </param>
    /// <param name="reset">
    /// A flag that indicates if the metric's internal state should be reseted
    /// for the next step.
    /// </param>
    void GetMeasure<T>(Action<Measure, T> callback, T state, bool reset);
  }
}
