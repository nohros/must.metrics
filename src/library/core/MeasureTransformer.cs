using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A implementation of the <see cref="IMetric"/> that apply a transformation
  /// function over a measure value.
  /// </summary>
  public class MeasureTransformer : IMetric
  {
    readonly IMetric metric_;
    readonly Func<double, double> transform_;

    /// <summary>
    /// Initializes a new instance of the <see cref="MeasureTransformer"/> by
    /// using the given metric and transform function.
    /// </summary>
    /// <param name="metric">
    /// The metric which measured's valued will be transformed.
    /// </param>
    /// <param name="transform">
    /// The <see cref="Func{T1, TResult}"/> that should be used to transform
    /// the measured values.
    /// </param>
    public MeasureTransformer(IMetric metric, Func<double, double> transform) {
      metric_ = metric;
      transform_ = transform;
    }

    /// <inheritdoc/>
    public void GetMeasure(Action<Measure> callback) {
      metric_.GetMeasure(
        measure =>
          callback(
            new Measure(metric_.Config, transform_(measure.Value),
              measure.IsObservable)));
    }

    /// <inheritdoc/>
    public void GetMeasure<T>(Action<Measure, T> callback, T state) {
      metric_.GetMeasure(
        measure =>
          callback(
            new Measure(metric_.Config, transform_(measure.Value),
              measure.IsObservable), state));
    }

    public MetricConfig Config {
      get { return metric_.Config; }
    }
  }
}
