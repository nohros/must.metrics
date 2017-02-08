using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A <see cref="IGauge"/> that computes its value using a
  /// <see cref="Func{T}"/>.
  /// </summary>
  public class CallableGauge : AbstractMetric, IGauge
  {
    protected readonly Func<Measure> callable_;

    /// <summary>
    /// Initializes a new instance of the <see cref="Func{T}"/>
    /// by using the specified <see cref="Func{T}"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    protected CallableGauge(MetricConfig config) : base(config) {
      callable_ = () => { throw new NotImplementedException(); };
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Func{T}"/>
    /// by using the specified <see cref="Func{T}"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="callable">
    /// A <see cref="Func{T}"/> that is used to compute the gauge
    /// values.
    /// </param>
    public CallableGauge(MetricConfig config, Func<Measure> callable)
      : base(config) {
      callable_ = callable;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Func{T}"/>
    /// by using the specified <see cref="Func{T}"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="callable">
    /// A <see cref="Func{T}"/> that is used to compute the gauge
    /// values.
    /// </param>
    public CallableGauge(MetricConfig config, Func<double> callable)
      : base(config) {
      callable_ = () => CreateMeasure(callable());
    }

    /// <inheritdoc/>
    protected internal override Measure Compute(long tick) {
      return callable_();
    }
  }
}
