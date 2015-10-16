using System;
using Nohros.Metrics.Reporting;

namespace Nohros.Metrics
{
  /// <summary>
  /// Represents the instantaneous value of a metric at a given point in time.
  /// A measure should represent a valid
  /// </summary>
  public class Measure
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Measure"/> by using
    /// the given measured value.
    /// </summary>
    /// <param name="value">
    /// The measured value.
    /// </param>
    /// <param name="config">
    /// The <see cref="MetricConfig"/> object that has produced the measure.
    /// </param>
    public Measure(MetricConfig config, double value) : this(config, value,
      DateTime.MinValue) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Measure"/> by using
    /// the given measured value.
    /// </summary>
    /// <param name="value">
    /// The measured value.
    /// </param>
    /// <param name="config">
    /// The <see cref="MetricConfig"/> object that has produced the measure.
    /// </param>
    /// <param name="observable">
    /// A value that indicates if the measure shoud be dispatched to a
    /// <see cref="IMeasureObserver"/>.
    /// </param>
    public Measure(MetricConfig config, double value, bool observable)
      : this(config, value, DateTime.MinValue, observable) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Measure"/> by using
    /// the given measured value.
    /// </summary>
    /// <param name="value">
    /// The measured value.
    /// </param>
    /// <param name="config">
    /// The <see cref="MetricConfig"/> object that has produced the measure.
    /// </param>
    /// <param name="timestamp">
    /// The date and time when the measure was measured
    /// </param>
    public Measure(MetricConfig config, double value, DateTime timestamp)
      : this(config, value, timestamp, true) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Measure"/> by using
    /// the given measured value.
    /// </summary>
    /// <param name="value">
    /// The measured value.
    /// </param>
    /// <param name="config">
    /// The <see cref="MetricConfig"/> object that has produced the measure.
    /// </param>
    /// <param name="observable">
    /// A value that indicates if the measure shoud be dispatched to a
    /// <see cref="IMeasureObserver"/>.
    /// </param>
    /// <param name="timestamp">
    /// The date and time when the measure was measured
    /// </param>
    public Measure(MetricConfig config, double value, DateTime timestamp,
      bool observable) {
      MetricConfig = config;
      Value = value;
      IsObservable = observable;
      TimeStamp = timestamp;
    }

    /// <summary>
    /// Gets the <see cref="MetricConfig"/> that is associated with the object
    /// that has produced the measure.
    /// </summary>
    public MetricConfig MetricConfig { get; private set; }

    /// <summary>
    /// Gets the instantaneous metric's value
    /// </summary>
    public double Value { get; private set; }

    /// <summary>
    /// Gets the date and time when the measure was measured.
    /// </summary>
    /// <remarks>
    /// This value is set only for measures of metrics that measure past
    /// events. Metrics that always measure current events should set the
    /// value of <see cref="TimeStamp"/> <see cref="DateTime.MinValue"/>.
    /// </remarks>
    public DateTime TimeStamp { get; private set; }

    /// <summary>
    /// Gets a value indicating if <see cref="Value"/> is observable, which
    /// means that it contains a value that could be dispatch to a
    /// <see cref="IMeasureObserver"/>.
    /// </summary>
    /// <remarks>
    /// Some <see cref="ICompositeMetric"/> could contains metrics that is
    /// not observable at a given point in time. For example, the
    /// <see cref="BucketTimer"/> contains metrics that reports how many times
    /// a given call took a specific time. For that composite reporting the
    /// value of the buckets that was never updated is a waste of time and
    /// space. The 'never updated buckets' will set the
    /// <see cref="IsObservable"/> property of the metrics that it report to
    /// <c>false</c> and the observables could just ignore it.
    /// </remarks>
    public bool IsObservable { get; private set; }
  }
}
