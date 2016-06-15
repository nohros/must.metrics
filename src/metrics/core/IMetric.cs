using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// Defines a class the is used to sample values tied to a particular
  /// <see cref="MetricConfig"/>.
  /// </summary>
  public interface IMetric
  {
    /// <summary>
    /// Gets the current measure for the given metric.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="Action"/> that should be called when the current measure
    /// for the current metric was computed.
    /// </param>
    void GetMeasure(Action<Measure> callback);

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
    void GetMeasure<T>(Action<Measure, T> callback, T state);

    /// <summary>
    /// Gets or sets the associated <see cref="MetricConfig"/> that is used to
    /// identify and provide metadata for a <see cref="IMetric"/>.
    /// </summary>
    MetricConfig Config { get; }
  }
}
