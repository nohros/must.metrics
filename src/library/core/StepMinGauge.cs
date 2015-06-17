using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// Defines a <see cref="IGauge"/> that keeps track of the minimum value
  /// seen since the last reset and is mapped to a particular step interval.
  /// </summary>
  /// <remarks>
  /// Updates should be non-negative. The reset value is
  /// <see cref="long.MaxValue"/>.
  /// </remarks>
  public class StepMinGauge : IMetric, IStepMetric
  {
    readonly MinGauge min_gauge_;

    /// <summary>
    /// Initializes a new instance of the <see cref="StepMinGauge"/>
    /// class by using the given <see cref="MetricConfig"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> object containing the configuration
    /// that shoud be used by the <see cref="StepMaxGauge"/> object.
    /// </param>
    public StepMinGauge(MetricConfig config) {
      min_gauge_ = new MinGauge(config);
    }

    /// <inheritdoc/>
    protected internal Measure Compute(long tick) {
      return min_gauge_.Compute(tick);
    }

    /// <inheritdoc/>
    public MetricConfig Config {
      get { return min_gauge_.Config; }
    }

    /// <inheritdoc/>
    public void GetMeasure(Action<Measure> callback) {
      min_gauge_.GetMeasure(callback);
    }

    /// <inheritdoc/>
    public void GetMeasure<T>(Action<Measure, T> callback, T state) {
      min_gauge_.GetMeasure(callback, state);
    }

    /// <inheritdoc/>
    public void GetMeasure(Action<Measure> callback, bool reset) {
      min_gauge_.GetMeasure(measure => {
        if (reset) {
          min_gauge_.Reset();
        }
        callback(measure);
      });
    }

    /// <inheritdoc/>
    public void GetMeasure<T>(Action<Measure, T> callback, T state, bool reset) {
      min_gauge_.GetMeasure((measure, state2) => {
        if (reset) {
          min_gauge_.Reset();
        }
        callback(measure, state2);
      }, state);
    }

    /// <inheritdoc/>
    public void Update(long v) {
      min_gauge_.Update(v);
    }
  }
}
