using System;

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

    public void OnStep() {
      Reset();
    }

    public void Reset() {
      max_gauge_.Reset();
    }

    public void Update(long v) {
      max_gauge_.Update(v);
    }
  }
}
