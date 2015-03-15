using System;
using System.Collections;
using System.Collections.Generic;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  public class Histogram : AbstractMetric, IHistogram, ICompositeMetric
  {
    class CallableGaugeWrapper
    {
      readonly MetricConfig config_;
      readonly Func<Snapshot, double> callable_;

      public CallableGaugeWrapper(MetricConfig config,
        Func<Snapshot, double> callable) {
        config_ = config;
        callable_ = callable;
      }

      public IMetric Wrap(Snapshot snapshot) {
        return new CallableGauge(config_, () => callable_(snapshot));
      }
    }

    readonly IResevoir resevoir_;
    readonly List<CallableGaugeWrapper> gauges_;

    /// <summary>
    /// Initializes a new instance of the <see cref="Histogram"/> that uses
    /// the <see cref="ExponentiallyDecayingResevoir"/> as resevoir.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="stats">
    /// A <see cref="SnapshotConfig"/> that defines the statistics that should
    /// be computed.
    /// </param>
    public Histogram(MetricConfig config, SnapshotConfig stats)
      : this(config, stats, new ExponentiallyDecayingResevoir(),
        MetricContext.ForCurrentProcess) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Histogram"/> by using the
    /// given <see cref="IResevoir"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="stats">
    /// A <see cref="SnapshotConfig"/> that defines the statistics that should
    /// be computed.
    /// </param>
    /// <param name="resevoir">
    /// A <see cref="IResevoir"/> that can be used to store the computed
    /// values.
    /// </param>
    public Histogram(MetricConfig config, SnapshotConfig stats,
      IResevoir resevoir)
      : this(config, stats, resevoir, MetricContext.ForCurrentProcess) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Histogram"/> by using the
    /// given <see cref="IResevoir"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="stats">
    /// A <see cref="SnapshotConfig"/> that defines the statistics that should
    /// be computed.
    /// </param>
    /// <param name="resevoir">
    /// A <see cref="IResevoir"/> that can be used to store the computed
    /// values.
    /// </param>
    /// <param name="context">
    /// A <see cref="MetricContext"/> that contains the shared
    /// <see cref="Mailbox{T}"/> and <see cref="Clock"/>.
    /// </param>
    public Histogram(MetricConfig config, SnapshotConfig stats,
      IResevoir resevoir, MetricContext context)
      : base(config, context) {
      resevoir_ = resevoir;

      gauges_ = new List<CallableGaugeWrapper>();

      if (stats.ComputeCount) {
        gauges_.Add(CountGauge(config));
      }

      if (stats.ComputeMax) {
        gauges_.Add(MaxGauge(config));
      }

      if (stats.ComputeMean) {
        gauges_.Add(MeanGauge(config));
      }

      if (stats.ComputeMedian) {
        gauges_.Add(MedianGauge(config));
      }

      if (stats.ComputeMin) {
        gauges_.Add(MinGauge(config));
      }

      if (stats.ComputeStdDev) {
        gauges_.Add(StdDevGauge(config));
      }

      foreach (var percentile in stats.Percentiles) {
        gauges_.Add(PercentileGauge(config, percentile));
      }
    }

    CallableGaugeWrapper MinGauge(MetricConfig config) {
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic", "min"),
          snapshot => snapshot.Min);
    }

    CallableGaugeWrapper MedianGauge(MetricConfig config) {
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic", "median"),
          snapshot => snapshot.Median);
    }

    CallableGaugeWrapper MeanGauge(MetricConfig config) {
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic", "mean"),
          snapshot => snapshot.Mean);
    }

    CallableGaugeWrapper MaxGauge(MetricConfig config) {
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic", "max"),
          snapshot => snapshot.Max);
    }

    CallableGaugeWrapper CountGauge(MetricConfig config) {
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic", "count"),
          snapshot => snapshot.Size);
    }

    CallableGaugeWrapper StdDevGauge(MetricConfig config) {
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic", "stddev"),
          snapshot => snapshot.StdDev);
    }

    CallableGaugeWrapper PercentileGauge(MetricConfig config, double percentile) {
      // We need to divide the percentile to 100 because the percentiles from
      // the SnapshotConfig is in range 0 to 100 and the Snapshot.Quantile
      // methof computes percentiles in range 0.0 to 1.0.
      return
        new CallableGaugeWrapper(
          config.WithAdditionalTag("statistic",
            "percentile_" + (percentile).ToString("#0.####")),
          snapshot => snapshot.Quantile(percentile/100));
    }

    /// <inheritdoc/>
    public void Update(long value) {
      long timestamp = resevoir_.Timestamp;
      context_.Send(() => Update(value, timestamp));
    }

    public void Update(long value, long timestamp) {
      context_.Send(() => resevoir_.Update(value, timestamp));
    }

    /// <inheritdoc/>
    public ICollection<IMetric> Metrics {
      get {
        Snapshot snapshot = resevoir_.Snapshot;
        var metrics = new IMetric[gauges_.Count];
        for (int i = 0; i < gauges_.Count; i++) {
          metrics[i] = gauges_[i].Wrap(snapshot);
        }
        return metrics;
      }
    }

    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    public IEnumerator<IMetric> GetEnumerator() {
      return Metrics.GetEnumerator();
    }

    /// <inheritdoc/>
    protected internal override Measure Compute(long tick) {
      return CreateMeasure(gauges_.Count);
    }
  }
}
