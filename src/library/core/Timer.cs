using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Nohros.Extensions.Time;

namespace Nohros.Metrics
{
  /// <summary>
  /// A simple <see cref="ITimer"/> implementation providing the total time,
  /// count, min and max for the times that have been recorded.
  /// </summary>
  public class Timer : AbstractMetric, ICompositeMetric, ITimer
  {
    const string kStatistic = "statistic";
    const string kTotal = "total";
    const string kCount = "count";
    const string kMin = "min";
    const string kMax = "max";

    readonly TimeUnit unit_;
    readonly Counter count_;
    readonly Counter total_time_;
    readonly StepMaxGauge max_;
    readonly StepMinGauge min_;
    readonly ReadOnlyCollection<IMetric> metrics_;

    /// <summary>
    /// Initializes a new instance of the <see cref="Timer"/> class by using
    /// the given configuration.
    /// </summary>
    /// <param name="config">
    /// </param>
    public Timer(MetricConfig config) : this(config, TimeUnit.Milliseconds) {
    }

    public Timer(MetricConfig config, TimeUnit unit)
      : this(config, unit, MetricContext.ForCurrentProcess) {
    }

    public Timer(MetricConfig config, TimeUnit unit, MetricContext context)
      : base(config, context) {
      unit_ = unit;

      MetricConfig cfg = config.WithAdditionalTag("unit", unit.Name());

      count_ = new Counter(cfg.WithAdditionalTag(kStatistic, kCount), context);
      max_ = new StepMaxGauge(cfg.WithAdditionalTag(kStatistic, kMax));
      min_ = new StepMinGauge(cfg.WithAdditionalTag(kStatistic, kMin));
      total_time_ =
        new Counter(cfg.WithAdditionalTag(kStatistic, kTotal), context);

      metrics_ = new ReadOnlyCollection<IMetric>(
        new IMetric[] {
          count_,
          new MeasureTransformer(max_, ConvertToUnit),
          new MeasureTransformer(min_, ConvertToUnit),
          new MeasureTransformer(total_time_, ConvertToUnit)
        });
    }

    /// <summary>
    /// Creates a new timer by using the specified name and the default
    /// values for resevoir, time unit and snapshot config.
    /// </summary>
    /// <param name="name">
    /// The name of the timer
    /// </param>
    /// <returns>
    /// A <see cref="StatsTimer"/> whose name is <paramref name="name"/> and uses
    /// the default resevoir, time unit and snapshot config.
    /// </returns>
    public static Timer Create(string name) {
      return new Timer(new MetricConfig(name));
    }

    /// <summary>
    /// Creates a new timer by using the specified name and time unit and the
    /// default values for resevoir and snapshot config.
    /// </summary>
    /// <param name="name">
    /// The name of the timer
    /// </param>
    /// <param name="unit">
    /// </param>
    /// <returns>
    /// A <see cref="StatsTimer"/> whose name is <paramref name="name"/> and uses
    /// the specified time unit and default resevoir and snapshot config.
    /// </returns>
    public static Timer Create(string name, TimeUnit unit) {
      return new Timer(new MetricConfig(name), unit);
    }

    /// <summary>
    /// Adds a recorded duration.
    /// </summary>
    /// <param name="duration">
    /// The length of the duration.
    /// </param>
    public void Update(TimeSpan duration) {
      if (duration > TimeSpan.Zero) {
        context_.Send(() => {
          long ticks = duration.Ticks;
          total_time_.Increment(ticks);
          count_.Increment();
          min_.Update(ticks);
          max_.Update(ticks);
        });
      }
    }

    public T Time<T>(Func<T> method) {
      // The time should be mensured even if a exception is throwed.
      var timer = new Stopwatch();
      try {
        timer.Start();
        return method();
      } finally {
        timer.Stop();
        Update(timer.Elapsed);
      }
    }

    public void Time(Action method) {
      // The time should be mensured even if a exception is throwed.
      var timer = new Stopwatch();
      try {
        timer.Start();
        method();
      } finally {
        timer.Stop();
        Update(timer.Elapsed);
      }
    }

    /// <summary>
    /// Gets a timing <see cref="TimerContext"/>, which measures an elapsed
    /// time using the same duration as the parent <see cref="StatsTimer"/>.
    /// </summary>
    /// <returns>
    /// A new <see cref="TimerContext"/>.
    /// </returns>
    public TimerContext Time() {
      return new TimerContext(this);
    }

    /// <inheritdoc/>
    public IEnumerator<IMetric> GetEnumerator() {
      return metrics_.GetEnumerator();
    }

    double ConvertToUnit(double d) {
      return TimeUnit.Ticks.Convert(d, unit_);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    /// <inheritdoc/>
    public ICollection<IMetric> Metrics {
      get { return metrics_; }
    }

    /// <inheritdoc/>
    protected internal override Measure Compute(long tick) {
      return CreateMeasure(metrics_.Count);
    }
  }
}
