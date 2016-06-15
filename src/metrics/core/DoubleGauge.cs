using System;
using System.Collections.Generic;

namespace Nohros.Metrics
{
  /// <summary>
  /// Defines a <see cref="IGauge"/> that reports a <see cref="double"/> value.
  /// </summary>
  public class DoubleGauge : AbstractMetric
  {
    double value_;
    DateTime timestamp_;

    /// <summary>
    /// Initializes a new instance of the <see cref="DoubleGauge"/> class by
    /// using the given <paramref name="config"/> object.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings for
    /// the metric.
    /// </param>
    public DoubleGauge(MetricConfig config)
      : base(config.WithAdditionalTag(MetricType.Gauge.AsTag())) {
    }

    /// <summary>
    /// Creates a new double gauge by uing the specified name.
    /// </summary>
    /// <param name="name">
    /// The name of the metric.
    /// </param>
    /// <returns></returns>
    public static DoubleGauge Create(string name) {
      return new DoubleGauge(new MetricConfig(name));
    }

    /// <summary>
    /// Creates a new double gauge by uing the specified name.
    /// </summary>
    /// <param name="name">
    /// The name of the metric.
    /// </param>
    /// <param name="tags">
    /// The tags that should be associated with the <see cref="DoubleGauge"/>.
    /// </param>
    /// <returns></returns>
    public static DoubleGauge Create(string name, Tags tags) {
      return new DoubleGauge(new MetricConfig(name).WithAdditionalTags(tags));
    }

    /// <summary>
    /// Creates a new double gauge by uing the specified name.
    /// </summary>
    /// <param name="name">
    /// The name of the metric.
    /// </param>
    /// <param name="tags">
    /// The tags that should be associated with the <see cref="DoubleGauge"/>.
    /// </param>
    /// <returns></returns>
    public static DoubleGauge Create(string name, IEnumerable<Tag> tags) {
      return new DoubleGauge(new MetricConfig(name).WithAdditionalTags(tags));
    }

    /// <summary>
    /// Set the current value.
    /// </summary>
    /// <param name="v"></param>
    public void Update(double v) {
      DateTime timestamp = DateTime.Now;
      context_.Send(() => Update(v, timestamp));
    }

    /// <summary>
    /// Set the current value.
    /// </summary>
    /// <param name="v"></param>
    /// <param name="timestamp"></param>
    public void Update(double v, DateTime timestamp) {
      context_.Send(() => {
        value_ = v;
        timestamp_ = timestamp;
      });
    }

    /// <inheritdoc/>
    protected internal override Measure Compute(long tick) {
      return CreateMeasure(value_, timestamp_);
    }
  }
}
