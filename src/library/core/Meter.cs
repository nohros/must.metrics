using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Nohros.Concurrent;
using Nohros.Extensions.Time;

namespace Nohros.Metrics
{
  /// <summary>
  /// A meter metric which measures mean throughput and one-, five-, and
  /// fifteen-minute exponetially-weighted moving average throughputs.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The value is the mean count per time unit within the specified interval.
  /// </para>
  /// <para>
  ///   http://en.wikipedia.org/wiki/Moving_average#Exponential_moving_average
  /// </para>
  /// </remarks>
  public class Meter : AbstractMetric, IMeter, ICompositeMetric
  {
    readonly ExponentialWeightedMovingAverage ewma_15_rate_;
    readonly ExponentialWeightedMovingAverage ewma_1_rate_;
    readonly ExponentialWeightedMovingAverage ewma_5_rate_;
    readonly ReadOnlyCollection<IMetric> metrics_;
    readonly MeanRate mean_rate_;

    /// <summary>
    /// Initializes a new instance of the <see cref=" Meter"/> class by using
    /// the specified config and rate unit.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="rate_unit">
    /// The time unit of the meter's rate.
    /// </param>
    public Meter(MetricConfig config, TimeUnit rate_unit)
      : this(config, rate_unit, MetricContext.ForCurrentProcess) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref=" Meter"/> class by using
    /// the specified meter name, rate unit and clock.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="rate_unit">
    /// The time unit of the meter's rate.
    /// </param>
    /// <param name="context">
    /// A <see cref="MetricContext"/> that contains the shared
    /// <see cref="Mailbox{T}"/> and <see cref="Clock"/>.
    /// </param>
    internal Meter(MetricConfig config, TimeUnit rate_unit,
      MetricContext context) : base(config, context) {
      const string kStatistic = "statistic";

      mean_rate_ = new MeanRate(
        config.WithAdditionalTag(new Tag(kStatistic, "mean_rate")), rate_unit,
        context);

      ewma_1_rate_ = ExponentialWeightedMovingAverage
        .ForOneMinute(
          config.WithAdditionalTag(new Tag(kStatistic, "ewma_m1_rate")),
          rate_unit, context);

      ewma_5_rate_ = ExponentialWeightedMovingAverage
        .ForFiveMinutes(
          config.WithAdditionalTag(new Tag(kStatistic, "ewma_m5_rate")),
          rate_unit, context);

      ewma_15_rate_ = ExponentialWeightedMovingAverage
        .ForFifteenMinutes(
          config.WithAdditionalTag(new Tag(kStatistic, "ewma_m15_rate")),
          rate_unit, context);

      metrics_ = new ReadOnlyCollection<IMetric>(
        new IMetric[] {
          mean_rate_, ewma_1_rate_, ewma_5_rate_, ewma_15_rate_
        });
    }

    /// <summary>
    /// Mark the occurrence of an event.
    /// </summary>
    public void Mark() {
      Mark(1);
    }

    /// <summary>
    /// Mark the occurrence of a given number of events.
    /// </summary>
    /// <param name="n">
    /// The number of events.
    /// </param>
    public void Mark(long n) {
      long timestamp = context_.Tick;

      // The mean_rate.Mauk method is asynchrnous, so we need to perform
      // it before the synchonrnous Mark method of the ewma rates.
      mean_rate_.Mark(n);
      context_.Send(() => Mark(n, timestamp));
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    /// <inheritdoc/>
    public IEnumerator<IMetric> GetEnumerator() {
      return metrics_.GetEnumerator();
    }

    /// <inheritdoc/>
    protected internal override Measure Compute(long tick) {
      return CreateMeasure(metrics_.Count);
    }

    /// <summary>
    /// Provides unsafe access to the internal Mark. This should be called
    /// using the same context that is used to update the histogram.
    /// </summary>
    internal void Mark(long n, long timestamp) {
      ewma_1_rate_.Mark(n, timestamp);
      ewma_5_rate_.Mark(n, timestamp);
      ewma_15_rate_.Mark(n, timestamp);
    }

    /// <inheritdoc/>
    public ICollection<IMetric> Metrics {
      get { return metrics_; }
    }
  }
}
