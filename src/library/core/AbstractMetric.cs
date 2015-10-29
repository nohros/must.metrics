using System;
using Nohros.Concurrent;
using Nohros.Metrics.Reporting;

namespace Nohros.Metrics
{
  /// <summary>
  /// An abstract implementation of the <see cref="IMetric"/> interface.
  /// </summary>
  public abstract class AbstractMetric : IMetric
  {
    protected internal MetricContext context_;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractMetric"/> class
    /// by using the given <paramref name="config"/> object.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    protected AbstractMetric(MetricConfig config)
      : this(config, MetricContext.ForCurrentProcess) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractMetric"/> class
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
    protected AbstractMetric(MetricConfig config, MetricContext context) {
      if (config == null || context == null) {
        throw new ArgumentNullException(config == null ? "config" : "context");
      }
      Config = config;
      context_ = context;
    }

    /// <inheritdoc/>
    public virtual void GetMeasure(Action<Measure> callback) {
      long tick = context_.Tick;
      DateTime now = DateTime.Now;
      context_.Send(() => callback(Compute(tick, now)));
    }

    /// <inheritdoc/>
    public virtual void GetMeasure<T>(Action<Measure, T> callback, T state) {
      long tick = context_.Tick;
      DateTime now = DateTime.Now;
      context_.Send(() => callback(Compute(tick, now), state));
    }

    /// <summary>
    /// Creates a <see cref="Measure"/> by using <see cref="Config"/> and the
    /// given metric's value.
    /// </summary>
    /// <param name="measure">
    /// The value of the measure.
    /// </param>
    /// <returns>
    /// A <see cref="Measure"/> containg the current metric's value.
    /// </returns>
    protected virtual Measure CreateMeasure(double measure) {
      return new Measure(Config, measure);
    }

    /// <summary>
    /// Creates a <see cref="Measure"/> by using <see cref="Config"/> and the
    /// given metric's value and observable state.
    /// </summary>
    /// <param name="measure">
    /// The value of the measure.
    /// </param>
    /// <param name="observable">
    /// A value that indicates if the created measure shoud be dispatched to a
    /// <see cref="IMeasureObserver"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Measure"/> containg the current metric's value.
    /// </returns>
    protected virtual Measure CreateMeasure(double measure, bool observable) {
      return new Measure(Config, measure, observable);
    }

    /// <summary>
    /// Creates a <see cref="Measure"/> by using <see cref="Config"/> and the
    /// given metric's value.
    /// </summary>
    /// <param name="measure">
    /// The value of the measure.
    /// </param>
    /// <param name="timestamp">
    /// The date and time when the <seealso cref="GetMeasure"/> was called.
    /// </param>
    /// <returns>
    /// A <see cref="Measure"/> containg the current metric's value.
    /// </returns>
    /// <remarks>
    /// The date and time when a measure was measured should be atached to
    /// a measure to allow past measures to be correctly reported, this
    /// parameter is used only by metrics that can be measured in the past.
    /// For metrics that does not support this behavior the value of
    /// <paramref name="timestamp"/> should be equals to
    /// <see cref="DateTime.MinValue"/>.
    /// </remarks>
    /// <see cref="CreateMeasure(double)"/>
    protected virtual Measure CreateMeasure(double measure, DateTime timestamp) {
      return new Measure(Config, measure, timestamp);
    }

    /// <summary>
    /// Creates a <see cref="Measure"/> by using <see cref="Config"/> and the
    /// given metric's value.
    /// </summary>
    /// <param name="measure">
    /// The value of the measure.
    /// </param>
    /// <param name="timestamp">
    /// The date and time when the <seealso cref="GetMeasure"/> was called.
    /// </param>
    /// <param name="observable">
    /// A value that indicates if the created measure shoud be dispatched to a
    /// <see cref="IMeasureObserver"/>.
    /// </param>
    /// <returns>
    /// A <see cref="Measure"/> containg the current metric's value.
    /// </returns>
    /// <remarks>
    /// The date and time when a measure was measured should be atached to
    /// a measure to allow past measures to be correctly reported, this
    /// parameter is used only by metrics that can be measured in the past.
    /// For metrics that does not support this behavior the value of
    /// <paramref name="timestamp"/> should be equals to
    /// <see cref="DateTime.MinValue"/>.
    /// </remarks>
    /// <see cref="CreateMeasure(double)"/>
    protected virtual Measure CreateMeasure(double measure, DateTime timestamp,
      bool observable) {
      return new Measure(Config, measure, timestamp, observable);
    }

    /// <summary>
    /// Computes the current value of a metric, synchrosnouly.
    /// </summary>
    /// <param name="tick">
    /// The value of <see cref="Clock.Tick"/> for the clock
    /// associated with the current context for the time when the
    /// <see cref="GetMeasure"/> method was called.
    /// </param>
    /// <returns>
    /// A <see cref="Measure"/> containg the current metric's value.
    /// </returns>
    /// <remarks>
    /// Due to the async nature of the metrics methods the
    /// <see cref="Compute(long)"/> could be called some time later than the
    /// time when the <paramref name="tick"/> was called. If a metric rely on
    /// the value of <see cref="Clock.Tick"/> property to compute the measured
    /// value, this delay could produce wrong measures. To avoid this a
    /// <see cref="IMetric"/> should use the the value of the
    /// <paramref name="tick"/> as the replacement for the
    /// <see cref="Clock.Tick"/>.
    /// <para>
    /// The <see cref="Compute(long)"/> method is executed inside the context
    /// of the associated <see cref="MetricContext"/>.
    /// </para>
    /// </remarks>
    protected internal abstract Measure Compute(long tick);

    /// <summary>
    /// Computes the current value of a metric, synchrosnouly.
    /// </summary>
    /// <param name="tick">
    /// The value of <see cref="Clock.Tick"/> for the clock
    /// associated with the current context for the time when the
    /// <see cref="GetMeasure"/> method was called.
    /// </param>
    /// <param name="time">
    /// The date and time when the <see cref="GetMeasure"/> method was called.
    /// </param>
    /// <returns>
    /// A <see cref="Measure"/> containg the current metric's value.
    /// </returns>
    /// <remarks>
    /// Due to the async nature of the metrics methods the
    /// <see cref="Compute(long, DateTime)"/> could be called some time later
    /// than the time when the <paramref name="tick"/> was called. If a metric
    /// rely on the value of <see cref="Clock.Tick"/> property or the
    /// <see cref="DateTime.Now"/> to compute the measured value, this delay
    /// could produce wrong measures. To avoid this a <see cref="IMetric"/>
    /// should use the the value of the <paramref name="tick"/> as the
    /// replacement for the <see cref="Clock.Tick"/>.
    /// <para>
    /// The <see cref="Compute(long, DateTime)"/> method is executed inside
    /// the context of the associated <see cref="MetricContext"/>.
    /// </para>
    /// </remarks>
    protected internal virtual Measure Compute(long tick, DateTime time) {
      return Compute(tick);
    }

    /// <inheritdoc/>
    public MetricConfig Config { get; private set; }
  }
}
