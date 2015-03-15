using System;
using Nohros;
using Nohros.Concurrent;

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
      context_.Send(() => callback(Compute(tick)));
    }

    /// <inheritdoc/>
    public virtual void GetMeasure<T>(Action<Measure, T> callback, T state) {
      long tick = context_.Tick;
      context_.Send(() => callback(Compute(tick), state));
    }

    /// <summary>
    /// Creates a <see cref="Measure"/> by using <see cref="Config"/> and the
    /// given metric's value.
    /// </summary>
    /// <returns>
    /// A <see cref="Measure"/> containg the current metric's value.
    /// </returns>
    protected virtual Measure CreateMeasure(double measure) {
      return new Measure(Config, measure);
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
    /// <see cref="Compute"/> culd be called some time later than when the
    /// <paramref name="tick"/> was called. If a metric rely on the
    /// value of <see cref="Clock.Tick"/> property to perform some task
    /// to compute the measured value, this delay could produce wrong
    /// measures. To avoid this a <see cref="IMetric"/> should use the
    /// the value of the <paramref name="tick"/> as the replacement
    /// for the <see cref="Clock.Tick"/>.
    /// </remarks>
    protected internal abstract Measure Compute(long tick);

    /// <inheritdoc/>
    public MetricConfig Config { get; private set; }
  }
}
