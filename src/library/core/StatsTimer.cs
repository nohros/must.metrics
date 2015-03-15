using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Nohros.Extensions.Time;

namespace Nohros.Metrics
{
  /// <summary>
  /// A timer metric which aggregates timing durations and provides duration
  /// statistics, like throughput, average and standard deviation.
  /// <para>
  /// The statistics are computed over are sample of data that is produced by
  /// a <see cref="IResevoir"/> object.
  /// </para>
  /// <para>
  /// While the statistics collected are accurate for the machine where they
  /// are collected they will not be correct if they are aggregated across
  /// groups of machines. If that is a expected use-case a better approach
  /// is to use buckets that corresponds to different times. For example, you
  /// might have a counter that tracks how many calls took <c>20ms</c>, one
  /// for <c>[20ms, 500ms]</c>, and one for 500ms. This bucketing approach
  /// can be easily aggregated.
  /// </para>
  /// <seealso cref="BucketTimer"/>
  /// </summary>
  public class StatsTimer : AbstractMetric, ICompositeMetric, ITimer
  {
    public class Builder
    {
      public Builder(MetricConfig config) {
        Config = config;
        TimeUnit = TimeUnit.Seconds;
        Context = MetricContext.ForCurrentProcess;
        Resevoir = new ExponentiallyDecayingResevoir();
        SnapshotConfig = new SnapshotConfig.Builder().Build();
      }

      public Builder WithResevoir(IResevoir resevoir) {
        Resevoir = resevoir;
        return this;
      }

      public Builder WithTimeUnit(TimeUnit unit) {
        TimeUnit = unit;
        return this;
      }

      public Builder WithSnapshotConfig(SnapshotConfig config) {
        SnapshotConfig = config;
        return this;
      }

      public Builder WithContext(MetricContext context) {
        Context = context;
        return this;
      }

      public StatsTimer Build() {
        return new StatsTimer(this);
      }

      public MetricConfig Config { get; private set; }
      public IResevoir Resevoir { get; private set; }
      public TimeUnit TimeUnit { get; private set; }
      public SnapshotConfig SnapshotConfig { get; internal set; }

      internal MetricContext Context { get; private set; }
    }

    readonly TimeUnit unit_;
    readonly Histogram histogram_;
    readonly Counter count_;
    readonly ReadOnlyCollection<IMetric> metrics_;

    StatsTimer(Builder builder) : base(builder.Config, builder.Context) {
      unit_ = builder.TimeUnit;

      // remove the count from the histogram, because we need to add a
      // tag for the time unit and this tag will make no sense for count
      // values.
      var snapshot_config =
        new SnapshotConfig.Builder(builder.SnapshotConfig)
          .WithCount(false)
          .Build();

      MetricConfig unit_config = builder
        .Config
        .WithAdditionalTag("unit", unit_.Name());

      MetricContext context = builder.Context;

      histogram_ = new Histogram(unit_config, snapshot_config, builder.Resevoir,
        context);

      Tag tag = MetricType.Counter.AsTag();
      count_ = new Counter(unit_config.WithAdditionalTag(tag), context);

      metrics_ = new ReadOnlyCollection<IMetric>(
        new IMetric[] {
          count_, new CompositMetricTransformer(histogram_, ConvertToUnit)
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
    public static StatsTimer Create(string name) {
      return new Builder(new MetricConfig(name)).Build();
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
    public static StatsTimer Create(string name, TimeUnit unit) {
      return
        new Builder(new MetricConfig(name))
          .WithTimeUnit(unit)
          .Build();
    }

    /// <summary>
    /// Creates a new timer by using the specified name and resevoir and the
    /// default values for time unit and snapshot config.
    /// </summary>
    /// <param name="name">
    /// The name of the timer
    /// </param>
    /// <param name="reservoir">
    /// </param>
    /// <returns>
    /// A <see cref="StatsTimer"/> whose name is <paramref name="name"/> and uses
    /// the specified resevoir and default time unit and snapshot config.
    /// </returns>
    public static StatsTimer Create(string name, IResevoir reservoir) {
      return
        new Builder(new MetricConfig(name))
          .WithResevoir(reservoir)
          .Build();
    }

    ICollection<IMetric> ConvertToUnit(ICollection<IMetric> metrics) {
      return
        metrics
          .Select(m => new MeasureTransformer(m, ConvertToUnit))
          .Cast<IMetric>()
          .ToList();
    }

    double ConvertToUnit(double d) {
      return TimeUnit.Ticks.Convert(d, unit_);
    }

    /// <summary>
    /// Adds a recorded duration.
    /// </summary>
    /// <param name="duration">
    /// The length of the duration.
    /// </param>
    public void Update(TimeSpan duration) {
      long timestamp = context_.Tick;
      context_.Send(() => Update(duration, timestamp));
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

    void Update(TimeSpan duration, long timestamp) {
      if (duration > TimeSpan.Zero) {
        histogram_.Update(duration.Ticks, timestamp);
        count_.Increment();
      }
    }
  }
}
