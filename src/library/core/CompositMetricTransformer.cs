using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nohros.Metrics
{
  /// <summary>
  /// A implementation of the <see cref="ICompositeMetric"/> that forwards
  /// all the <see cref="IMetric"/> methods to another
  /// <see cref="ICompositeMetric"/> and transform the
  /// <see cref="ICompositeMetric.Metrics"/> in another collection using a
  /// <see cref="Func{T1, TResult}"/>.
  /// </summary>
  public class CompositMetricTransformer : ICompositeMetric
  {
    readonly ICompositeMetric metric_;
    readonly Func<ICollection<IMetric>, ICollection<IMetric>> transform_;

    public CompositMetricTransformer(ICompositeMetric metric,
      Func<ICollection<IMetric>, ICollection<IMetric>> transform) {
      metric_ = metric;
      transform_ = transform;
    }

    public MetricConfig Config {
      get { return metric_.Config; }
    }

    /// <summary>
    /// Gets the list of sub-metrics for the associated composite metric
    /// after its transformation.
    /// </summary>
    /// <value>
    /// The result of the transformation applied to the collection of
    /// sub-metrics of the associated composite metric.
    /// </value>
    public ICollection<IMetric> Metrics {
      get { return transform_(metric_.Metrics); }
    }

    /// <inheritdoc/>
    public void GetMeasure(Action<Measure> callback) {
      metric_.GetMeasure(callback);
    }

    /// <inheritdoc/>
    public void GetMeasure<T>(Action<Measure, T> callback, T state) {
      metric_.GetMeasure(callback, state);
    }

    /// <inheritdoc/>
    public IEnumerator<IMetric> GetEnumerator() {
      return Metrics.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }
  }
}
