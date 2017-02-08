using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  /// <summary>
  /// Simple composite metric with a static list of sub-metrics. The value of
  /// the composite metric is the number of sub-metrics.
  /// </summary>
  public class BasicCompositeMetric : AbstractMetric, ICompositeMetric
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="BasicCompositeMetric"/> by
    /// using the given list of sub-metrics.
    /// </summary>
    /// <param name="metrics">
    /// </param>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    public BasicCompositeMetric(MetricConfig config,
      IEnumerable<IMetric> metrics)
      : this(config, metrics, MetricContext.ForCurrentProcess) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BasicCompositeMetric"/> by
    /// using the given list of sub-metrics.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="metrics">
    /// </param>
    /// <param name="context">
    /// A <see cref="MetricContext"/> that contains the shared
    /// <see cref="Mailbox{T}"/> and <see cref="Clock"/>.
    /// </param>
    public BasicCompositeMetric(MetricConfig config,
      IEnumerable<IMetric> metrics, MetricContext context)
      : base(config, context) {
      Metrics = new ReadOnlyCollection<IMetric>(metrics.ToArray());
    }

    /// <inheritdoc/>
    protected internal override Measure Compute(long tick) {
      return CreateMeasure(Metrics.Count);
    }

    /// <inheritdoc/>
    public ICollection<IMetric> Metrics { get; private set; }

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
