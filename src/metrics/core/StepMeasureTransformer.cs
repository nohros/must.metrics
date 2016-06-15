using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A implementation of the <see cref="IMetric"/> interface that apply a transformation
  /// function over a measure value.
  /// </summary>
  public class StepMeasureTransformer : MeasureTransformer, IStepMetric
  {
    readonly IStepMetric metric_;
    readonly Func<double, double> transform_;

    public StepMeasureTransformer(IStepMetric metric,
      Func<double, double> transform)
      : base(metric, transform) {
      metric_ = metric;
      transform_ = transform;
    }

    public void GetMeasure(Action<Measure> callback, bool reset) {
      metric_.GetMeasure(
        measure =>
          callback(
            new Measure(metric_.Config, transform_(measure.Value),
              measure.IsObservable)), reset);
    }

    public void GetMeasure<T>(Action<Measure, T> callback, T state, bool reset) {
      metric_.GetMeasure(
        (measure, state2) =>
          callback(
            new Measure(metric_.Config, transform_(measure.Value),
              measure.IsObservable), state2), state, reset);
    }
  }
}
