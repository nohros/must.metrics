using System.Collections.Generic;
using System.Threading;

namespace Nohros.Metrics
{
  /// <summary>
  /// Defines a <see cref="IGauge"/> that reports a <see cref="long"/> value.
  /// </summary>
  public class LongGauge : AbstractMetric
  {
    long value_;

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
      Interlocked.Exchange(ref value_, v);
    }

    protected internal override Measure Compute(long tick) {
      long v = Interlocked.Read(ref value_);
      return CreateMeasure(v);
    }
  }
}
