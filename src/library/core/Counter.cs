using System;
using System.Collections.Generic;
using System.Threading;
using Nohros.Metrics.Reporting;

namespace Nohros.Metrics
{
  /// <summary>
  /// A simple counter implementation of the <see cref="ICounter"/> class.
  /// </summary>
  /// <remarks>
  /// The value is the total count for the life of the counter.
  /// <see cref="IMeasureObserver"/>s are responsible for converting to a rate
  /// and handling overflows if they occur.
  /// </remarks>
  /// <see cref="StepCounter"/>
  public class Counter : AbstractMetric, ICounter
  {
    long count_;

    /// <summary>
    /// Initializes a new instance of the <see cref="Counter"/> class that
    /// uses the specified executor to perform the counter updates (
    /// increment/decrement).
    /// </summary>
    public Counter(MetricConfig config) : this(config, 0) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Counter"/> class.
    /// </summary>
    /// <param name="config"></param>
    /// <param name="context"></param>
    public Counter(MetricConfig config, MetricContext context)
      : this(config, 0, context) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Counter"/> class that
    /// uses the specified executor to perform the counter updates (
    /// increment/decrement).
    /// </summary>
    public Counter(MetricConfig config, long initial)
      : this(config, initial, MetricContext.ForCurrentProcess) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Counter"/> class that
    /// uses the specified executor to perform the counter updates (
    /// increment/decrement).
    /// </summary>
    public Counter(MetricConfig config, long initial, MetricContext context)
      : base(config.WithAdditionalTag(MetricType.Counter.AsTag()), context) {
      count_ = initial;
    }

    /// <inheritdoc/>
    public void Increment() {
      Increment(1);
    }

    /// <inheritdoc/>
    public void Increment(long n) {
      Update(n);
    }

    /// <inheritdoc/>
    public override void GetMeasure(Action<Measure> callback) {
      long tick = context_.Tick;
      callback(Compute(tick));
    }

    /// <inheritdoc/>
    public override void GetMeasure<T>(Action<Measure, T> callback, T state) {
      long tick = context_.Tick;
      callback(Compute(tick), state);
    }

    /// <summary>
    /// Creates a new counter by using the specified name.
    /// </summary>
    /// <param name="name">
    /// The name of the metric.
    /// </param>
    public static Counter Create(string name) {
      return new Counter(new MetricConfig(name));
    }

    /// <summary>
    /// Creates a new counter by using the specified name.
    /// </summary>
    /// <param name="name">
    /// The name of the metric.
    /// </param>
    /// <param name="tags">
    /// The tags that should be associated with the <see cref="Counter"/>.
    /// </param>
    public static Counter Create(string name, Tags tags) {
      return new Counter(new MetricConfig(name).WithAdditionalTags(tags));
    }

    /// <summary>
    /// Creates a new counter by using the specified name.
    /// </summary>
    /// <param name="name">
    /// The name of the metric.
    /// </param>
    /// <param name="tags">
    /// The tags that should be associated with the <see cref="Counter"/>.
    /// </param>
    public static Counter Create(string name, IEnumerable<Tag> tags) {
      return new Counter(new MetricConfig(name).WithAdditionalTags(tags));
    }

    /// <inheritdoc/>
    protected internal override Measure Compute(long tick) {
      return new Measure(Config, Count());
    }

    /// <summary>
    /// Gets the cuulative counter since the <see cref="Counter"/> was created.
    /// </summary>
    /// <returns>
    /// The cuulative counter since the <see cref="Counter"/> was created.
    /// </returns>
    public long Count() {
      return Interlocked.Read(ref count_);
    }

    void Update(long delta) {
      Interlocked.Add(ref count_, delta);
    }
  }
}
