using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A implementation of the <see cref="IMetric"/> interface that apply a transformation
  /// function over a measure value. The <see cref="StepMeasureTransformer"/> also implments
  /// the <see cref="IStepMetric"/> interface, so it can forward the
  /// <see cref="IStepMetric.OnStep"/> method call to the underlying metric.
  /// </summary>
  public class StepMeasureTransformer : MeasureTransformer, IStepMetric
  {
    readonly IStepMetric metric_;

    public StepMeasureTransformer(IStepMetric metric, Func<double, double> transform)
      : base(metric, transform) {
      metric_ = metric;
    }

    /// <inheritdoc/>
    public void OnStep() {
      metric_.OnStep();
    }
  }
}
