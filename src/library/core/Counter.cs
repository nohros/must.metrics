using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  /// <summary>
  /// A simple counter implementation of the <see cref="ICounter"/> class.
  /// </summary>
  /// <remarks>
  /// The value is the instantaneous value of the counter and it is never
  /// negative. If a <see cref="Decrement(long)"/> causes the counter
  /// do becomes negative its values will be set to zero.
  /// </remarks>
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

    /// <summary>
    /// Creates a new counter by using the specified name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Counter Create(string name) {
      return new Counter(new MetricConfig(name));
    }

    /// <inheritdoc/>
    public void Increment() {
      Increment(1);
    }

    /// <inheritdoc/>
    public void Increment(long n) {
      context_.Send(() => Update(n));
    }

    /// <inheritdoc/>
    public void Decrement() {
      Decrement(1);
    }

    /// <inheritdoc/>
    public void Decrement(long n) {
      context_.Send(() => Update(-n));
    }

    /// <inheritdoc/>
    protected internal override Measure Compute(long tick) {
      return CreateMeasure(count_);
    }

    void Update(long delta) {
      count_ += delta;
      if (count_ < 0) {
        count_ = 0;
      }
    }
  }
}
