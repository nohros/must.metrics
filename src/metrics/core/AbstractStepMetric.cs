using System;

namespace Nohros.Metrics
{
  public abstract class AbstractStepMetric : AbstractMetric, IStepMetric
  {
    protected AbstractStepMetric(MetricConfig config) : base(config) {
    }

    protected AbstractStepMetric(MetricConfig config, MetricContext context)
      : base(config, context) {
    }

    /// <summary>
    /// Computes the current value of a metric, synchrosnouly.
    /// </summary>
    /// <param name="tick">
    /// The value of <see cref="Clock.Tick"/> for the clock
    /// associated with the current context for the time when the
    /// <see cref="GetMeasure{T}"/> method was called.
    /// </param>
    /// <param name="reset">
    /// A flag that indicates if the metric's internal state should be reset
    /// for the next step.
    /// </param>
    /// <returns>
    /// A <see cref="Measure"/> containg the current metric's value.
    /// </returns>
    /// <remarks>
    /// Due to the async nature of the metrics methods the
    /// <see cref="Compute"/> culd be called some time later than when the
    /// <paramref name="tick"/> was called. If a metric rely on the
    /// value of <see cref="Clock.Tick"/> property to perform some task
    /// to compute the measured value, this delay could produce wrong
    /// measures. To avoid this a <see cref="IMetric"/> should use the
    /// the value of the <paramref name="tick"/> as the replacement
    /// for the <see cref="Clock.Tick"/>.
    /// </remarks>
    protected internal abstract Measure Compute(long tick, bool reset);

    /// <summary>
    /// Gets the current measure for the given metric.
    /// </summary>
    /// <param name="callback">
    /// A <see cref="Action"/> that should be called when the current measure
    /// for the current metric was computed.
    /// </param>
    /// <param name="reset">
    /// A flag that indicates if the metric's internal state should be reset
    /// for the next step.
    /// </param>
    public virtual void GetMeasure(Action<Measure> callback, bool reset) {
      long tick = context_.Tick;
      context_.Send(() => callback(Compute(tick, reset)));
    }

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
    /// A flag that indicates if the metric's internal state should be reset
    /// for the next step.
    /// </param>
    public virtual void GetMeasure<T>(Action<Measure, T> callback, T state,
      bool reset) {
      long tick = context_.Tick;
      context_.Send(() => callback(Compute(tick, reset), state));
    }
  }
}
