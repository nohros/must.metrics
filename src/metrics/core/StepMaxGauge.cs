using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  /// <summary>
  /// Defines a <see cref="IGauge"/> that keeps track of the maximum value
  /// seen since the last reset and is mapped to a particular step interval.
  /// </summary>
  /// <remarks>
  /// Updates should be non-negative. The reset value is zero.
  /// </remarks>
  public class StepMaxGauge : IMetric, IStepMetric
  {
    readonly MaxGauge max_gauge_;

    /// <summary>
    /// Initializes a new instance of the <see cref="StepMaxGauge"/>
    /// class by using the given <see cref="MetricConfig"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> object containing the configuration
    /// that shoud be used by the <see cref="StepMaxGauge"/> object.
    /// </param>
    public StepMaxGauge(MetricConfig config) {
      max_gauge_ = new MaxGauge(config);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StepMaxGauge"/> class
    /// by using the given <paramref name="config"/> object.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="context">
    /// A <see cref="MetricContext"/> that contains the shared
    /// <see cref="Mailbox{T}"/> and <see cref="Clock"/>.
    /// </param>
    public StepMaxGauge(MetricConfig config, MetricContext context) {
      max_gauge_ = new MaxGauge(config, context);
    }

    /// <inheritdoc/>
    public MetricConfig Config {
      get { return max_gauge_.Config; }
    }

    public void GetMeasure(Action<Measure> callback) {
      max_gauge_.GetMeasure(callback);
    }

    public void GetMeasure<T>(Action<Measure, T> callback, T state) {
      max_gauge_.GetMeasure(callback, state);
    }

    public void GetMeasure(Action<Measure> callback, bool reset) {
      max_gauge_.GetMeasure(measure => {
        if (reset) {
          max_gauge_.Reset();
        }
        callback(measure);
      });
    }

    public void GetMeasure<T>(Action<Measure, T> callback, T state, bool reset) {
      max_gauge_.GetMeasure((measure, state2) => {
        if (reset) {
          max_gauge_.Reset();
        }
        callback(measure, state2);
      }, state);
    }

    public void Update(long v) {
      max_gauge_.Update(v);
    }
  }
}
