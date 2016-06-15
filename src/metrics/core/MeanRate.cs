using System;
using Nohros.Concurrent;
using Nohros.Extensions.Time;

namespace Nohros.Metrics
{
  public class MeanRate : AbstractMetric, IMeter
  {
    readonly double ticks_per_unit_;
    readonly long start_time_;
    readonly Counter count_;

    /// <summary>
    /// Initializes a new instance of the <see cref=" MeanRate"/> class by
    /// using the specified config, rate unit and <see cref="StopwatchClock"/>
    /// as the clock.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="rate_unit">
    /// The time unit of the meter's rate.
    /// </param>
    public MeanRate(MetricConfig config, TimeUnit rate_unit)
      : this(config, rate_unit, MetricContext.ForCurrentProcess) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref=" MeanRate"/> class by using
    /// the specified meter name, rate unit and clock.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="unit">
    /// The time unit of the meter's rate.
    /// </param>
    /// <param name="context">
    /// A <see cref="MetricContext"/> that contains the shared
    /// <see cref="Mailbox{T}"/> and <see cref="Clock"/>.
    /// </param>
    public MeanRate(MetricConfig config, TimeUnit unit,
      MetricContext context) : base(config, context) {
      start_time_ = context_.Tick;
      count_ = new Counter(config, 0, context);
      ticks_per_unit_ = 1.ToTicks(unit);
    }

    /// <inheritdoc/>
    public void Mark() {
      Mark(1);
    }

    /// <inheritdoc/>
    public void Mark(long n) {
      count_.Increment(n);
    }

    /// <inheritdoc/>
    protected internal override Measure Compute(long tick) {
      Measure count = count_.Compute(tick);

      long elapsed = tick - start_time_;
      double rate = count.Value / elapsed;
      return CreateMeasure(rate * ticks_per_unit_);
    }
  }
}
