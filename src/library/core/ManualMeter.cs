using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A meter metric which measures mean throughput and one-, five-, and
  /// fifteen-minute exponetially-weighted moving average throughputs.
  /// </summary>
  /// <remarks>
  /// <para>
  ///   http://en.wikipedia.org/wiki/Moving_average#Exponential_moving_average
  /// </para>
  /// </remarks>
  public class ManualMeter : IMetric
  {
    const long kTickInterval = 5000000000; // 5 seconds in nanoseconds
    readonly ExponentialWeightedMovingAverage ewma_15_rate_;
    readonly ExponentialWeightedMovingAverage ewma_1_rate_;
    readonly ExponentialWeightedMovingAverage ewma_5_rate_;
    readonly TimeUnit rate_unit_;
    readonly long start_time_;
    long count_;
    bool ignore_old_events_;
    long last_mark_;
    long last_tick_;

    /// <summary>
    /// Initialize a new instance of the <see cref="Meter"/> class by using
    /// the given clock and <see cref="TimeUnit.Seconds"/> as event rate unit.
    /// </summary>
    /// <param name="start_time">
    /// The starting point.
    /// </param>
    public ManualMeter(long start_time)
      : this(TimeUnit.Seconds, start_time) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref=" Meter"/> class by using
    /// the specified meter name, rate unit and clock.
    /// </summary>
    /// <param name="start_time">
    /// The starting point.
    /// </param>
    /// <param name="rate_unit">
    /// The rate unit of the new meter.
    /// </param>
    public ManualMeter(TimeUnit rate_unit, long start_time) {
      count_ = 0;
      rate_unit_ = rate_unit;
      start_time_ = start_time;
      last_tick_ = start_time_;
      ewma_1_rate_ = ExponentialWeightedMovingAverages.OneMinute();
      ewma_5_rate_ = ExponentialWeightedMovingAverages.FiveMinute();
      ewma_15_rate_ = ExponentialWeightedMovingAverages.FifteenMinute();
      ignore_old_events_ = false;
    }

    /// <summary>
    /// Mark the occurrence of a given number of events.
    /// </summary>
    /// <param name="n">
    /// The number of events.
    /// </param>
    /// <param name="time">
    /// The time when the event has occured in nanoseconds.
    /// </param>
    public virtual void Mark(long n, long time) {
      if (time < last_mark_) {
        if (!ignore_old_events_) {
          throw new ArgumentOutOfRangeException(
            "You can not mark events that has been occured before the last marked.");
        }
      }
      TickIfNecessary(time);
      count_ += n;
      ewma_1_rate_.Update(n);
      ewma_5_rate_.Update(n);
      ewma_15_rate_.Update(n);
      last_mark_ = time;
    }

    public virtual double GetMeanRate(long timestamp) {
      if (count_ == 0) {
        return 0.0;
      }

      long elapsed = timestamp - start_time_;
      double rate = count_/(double) elapsed;
      return ConvertNsRate(rate);
    }

    /// <summary>
    /// Updates the moving average.
    /// </summary>
    void Tick() {
      ewma_1_rate_.Tick();
      ewma_5_rate_.Tick();
      ewma_15_rate_.Tick();
    }

    protected void TickIfNecessary(long now) {
      long age = now - last_tick_;
      if (age > kTickInterval) {
        last_tick_ = now - age%kTickInterval;
        long required_ticks = age/kTickInterval;
        for (long i = 0; i < required_ticks; i++) {
          Tick();
        }
      }
    }

    /// <inheritdoc/>
    protected double ConvertNsRate(double rate_per_ns) {
      return rate_per_ns*TimeUnitHelper.ToNanos(1, rate_unit_);
    }

    /// <summary>
    /// Gets the time associated with of the last marked event.
    /// </summary>
    public long LastMark {
      get { return last_mark_; }
    }

    /// <inheritdoc/>
    public TimeUnit RateUnit {
      get { return rate_unit_; }
    }

    /// <inheritdoc/>
    public virtual double FifteenMinuteRate {
      get { return ewma_15_rate_.Rate(rate_unit_); }
    }

    /// <inheritdoc/>
    public virtual double FiveMinuteRate {
      get { return ewma_5_rate_.Rate(rate_unit_); }
    }

    /// <inheritdoc/>
    public virtual double OneMinuteRate {
      get { return ewma_1_rate_.Rate(rate_unit_); }
    }

    /// <inheritdoc/>
    public long Count {
      get { return count_; }
    }

    /// <summary>
    /// Gets or sets a value indicating if events that has been occured before
    /// the last marked, should be ignored. The default is false.
    /// </summary>
    /// <value>
    /// <c>true</c> to ignore events that has been occured before
    /// the last marked; otherwise, <c>false</c>.
    /// </value>
    public virtual bool IgnoreOldEvets {
      get { return ignore_old_events_; }
      set { ignore_old_events_ = value; }
    }
  }
}
