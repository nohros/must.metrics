using System;
using System.Collections.Generic;

namespace Nohros.Metrics
{
  /// <summary>
  /// Defines a <see cref="IGauge"/> that reports a <see cref="long"/> value.
  /// </summary>
  public class LongGauge : AbstractMetric
  {
    long value_;
    DateTime timestamp_;

    /// <summary>
    /// Initializes a new instance of the <see cref="LongGauge"/> class by
    /// using the given <paramref name="config"/> object.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings for
    /// the metric.
    /// </param>
    public LongGauge(MetricConfig config)
      : base(config.WithAdditionalTag(MetricType.Gauge.AsTag())) {
      timestamp_ = DateTime.MinValue;
    }

    /// <summary>
    /// Creates a new long gauge by uing the specified name.
    /// </summary>
    /// <param name="name">
    /// The name of the metric.
    /// </param>
    /// <returns></returns>
    public static LongGauge Create(string name) {
      return new LongGauge(new MetricConfig(name));
    }

    /// <summary>
    /// Creates a new long gauge by uing the specified name.
    /// </summary>
    /// <param name="name">
    /// The name of the metric.
    /// </param>
    /// <param name="tags">
    /// The tags that should be associated with the <see cref="LongGauge"/>.
    /// </param>
    /// <returns></returns>
    public static LongGauge Create(string name, Tags tags) {
      return new LongGauge(new MetricConfig(name).WithAdditionalTags(tags));
    }

    /// <summary>
    /// Creates a new long gauge by uing the specified name.
    /// </summary>
    /// <param name="name">
    /// The name of the metric.
    /// </param>
    /// <param name="tags">
    /// The tags that should be associated with the <see cref="LongGauge"/>.
    /// </param>
    /// <returns></returns>
    public static LongGauge Create(string name, IEnumerable<Tag> tags) {
      return new LongGauge(new MetricConfig(name).WithAdditionalTags(tags));
    }

    /// <summary>
    /// Set the current value.
    /// </summary>
    /// <param name="v"></param>
    public void Update(long v) {
      DateTime timestamp = DateTime.Now;
      context_.Send(() => Update(v, timestamp));
    }

    /// <summary>
    /// Set the current value.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="timestamp"></param>
    public void Update(long v, DateTime timestamp) {
      context_.Send(() => {
        value_ = v;
        timestamp_ = timestamp;
      });
    }

    protected internal override Measure Compute(long tick) {
      return CreateMeasure(value_, timestamp_);
    }
  }
}
