using System;
using System.Threading;

namespace Nohros.Metrics
{
  /// <summary>
  /// Defines a <see cref="IGauge"/> that keeps track of the maximum value
  /// seen since the last reset.
  /// </summary>
  /// <remarks>
  /// Updates should be non-negative. The reset value is zero.
  /// </remarks>
  public class MaxGauge : AbstractMetric
  {
    long value_;

    /// <summary>
    /// Initializes a new instance of the <see cref="MaxGauge"/> class
    /// by using the given <paramref name="config"/> object.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    public MaxGauge(MetricConfig config)
      : base(config.WithAdditionalTag(MetricType.Gauge.AsTag())) {
    }

    /// <summary>
    /// Update the value of the gauge if the given value <paramref name="v"/>
    /// is larger than the current max.
    /// </summary>
    /// <param name="v">
    /// The value to be updated.
    /// </param>
    public void Update(long v) {
      long current = Interlocked.Read(ref value_);
      while (v > current) {
        if (Interlocked.CompareExchange(ref value_, v, current) == current) {
          break;
        }
        current = Interlocked.Read(ref value_);
      }
    }

    /// <inheritdoc/>
    public void Reset() {
      Interlocked.Exchange(ref value_, 0);
    }

    /// <inheritdoc/>
    protected internal override Measure Compute(long tick) {
      long v = Interlocked.Read(ref value_);
      return CreateMeasure(v);
    }
  }
}
