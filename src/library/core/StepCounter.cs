
using Nohros.Extensions.Time;

namespace Nohros.Metrics
{
  /// <summary>
  /// A <see cref="ICounter"/> implementation that is mapped to a particular
  /// step interval. The value returned is the rate for the previous interval
  /// as defined by the step.
  /// </summary>
  public class StepCounter : AbstractMetric, ICounter, IStepMetric
  {
    long prev_count_;
    long curr_count_;
    long prev_tick_;
    long curr_tick_;
    readonly TimeUnit unit_;

    /// <summary>
    /// Initializes a new instance of the <see cref="StepCounter"/> class
    /// by using the given <see cref="MetricConfig"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> object containing the configuration
    /// that should be used by the <see cref="StepCounter"/> object.
    /// </param>
    public StepCounter(MetricConfig config)
      : this(config, TimeUnit.Seconds) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StepCounter"/> class
    /// by using the given <see cref="MetricConfig"/>.
    /// </summary>
    /// <param name="unit">
    /// The unit to be used to report the computed rate.
    /// </param>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> object containing the configuration
    /// that should be used by the <see cref="StepCounter"/> object.
    /// </param>
    public StepCounter(MetricConfig config, TimeUnit unit)
      : this(config, 0, unit) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StepCounter"/> class
    /// by using the given <see cref="MetricConfig"/>,
    /// <see cref="MetricContext"/> and <paramref name="initial"/> value.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> object containing the configuration
    /// that should be used by the <see cref="StepCounter"/> object.
    /// </param>
    /// <param name="initial">
    /// The initial value of the counter.
    /// </param>
    public StepCounter(MetricConfig config, long initial)
      : this(config, initial, TimeUnit.Seconds) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StepCounter"/> class
    /// by using the given <see cref="MetricConfig"/>,
    /// <see cref="MetricContext"/> and <paramref name="initial"/> value.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> object containing the configuration
    /// that should be used by the <see cref="StepCounter"/> object.
    /// </param>
    /// <param name="unit">
    /// The unit to be used to report the computed rate.
    /// </param>
    /// <param name="initial">
    /// The initial value of the counter.
    /// </param>
    public StepCounter(MetricConfig config, long initial, TimeUnit unit)
      : this(config, initial, unit, MetricContext.ForCurrentProcess) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StepCounter"/> class
    /// by using the given <see cref="MetricConfig"/> and
    /// <see cref="MetricContext"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> object containing the configuration
    /// that should be used by the <see cref="StepCounter"/> object.
    /// </param>
    /// <param name="context">
    /// The <see cref="MetricContext"/> to be used by the counter.
    /// </param>
    public StepCounter(MetricConfig config, MetricContext context)
      : this(config, TimeUnit.Seconds, context) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StepCounter"/> class
    /// by using the given <see cref="MetricConfig"/> and
    /// <see cref="MetricContext"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> object containing the configuration
    /// that should be used by the <see cref="StepCounter"/> object.
    /// </param>
    /// <param name="unit">
    /// The unit to be used to report the computed rate.
    /// </param>
    /// <param name="context">
    /// The <see cref="MetricContext"/> to be used by the counter.
    /// </param>
    public StepCounter(MetricConfig config, TimeUnit unit, MetricContext context)
      : this(config, 0, unit, context) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StepCounter"/> class
    /// by using the given <see cref="MetricConfig"/>, initial value and
    /// <see cref="MetricContext"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> object containing the configuration
    /// that should be used by the <see cref="StepCounter"/> object.
    /// </param>
    /// <param name="initial">
    /// The initial value of the counter.
    /// </param>
    /// <param name="context">
    /// The <see cref="MetricContext"/> to be used by the counter.
    /// </param>
    public StepCounter(MetricConfig config, long initial, MetricContext context)
      : this(config, initial, TimeUnit.Seconds, context) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="StepCounter"/> class
    /// by using the given <see cref="MetricConfig"/>, initial value and
    /// <see cref="MetricContext"/>.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> object containing the configuration
    /// that should be used by the <see cref="StepCounter"/> object.
    /// </param>
    /// <param name="initial">
    /// The initial value of the counter.
    /// </param>
    /// <param name="unit">
    /// The unit to be used to report the computed rate.
    /// </param>
    /// <param name="context">
    /// The <see cref="MetricContext"/> to be used by the counter.
    /// </param>
    public StepCounter(MetricConfig config, long initial, TimeUnit unit,
      MetricContext context)
      : base(
        config
          .WithAdditionalTag(MetricType.Normalized.AsTag())
          .WithAdditionalTag("unit", unit.Name()), context) {
      prev_count_ = initial;
      curr_count_ = initial;
      prev_tick_ = curr_tick_ = context.Tick;
      unit_ = unit;
    }

    /// <summary>
    /// Creates a new counter by using the specified name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static StepCounter Create(string name) {
      return new StepCounter(new MetricConfig(name));
    }

    /// <summary>
    /// Creates a new counter by using the specified name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public static StepCounter Create(string name, TimeUnit unit) {
      return new StepCounter(new MetricConfig(name), unit);
    }

    /// <summary>
    /// Creates a new counter by using the specified name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="initial"></param>
    /// <returns></returns>
    public static StepCounter Create(string name, int initial) {
      return new StepCounter(new MetricConfig(name), initial);
    }

    /// <summary>
    /// Creates a new counter by using the specified name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="initial"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static StepCounter Create(string name, int initial, TimeUnit unit) {
      return new StepCounter(new MetricConfig(name), initial, unit);
    }

    /// <inheritdoc/>
    public void Decrement() {
      Decrement(-1);
    }

    /// <inheritdoc/>
    public void Decrement(long n) {
      context_.Send(() => Update(-n));
    }

    /// <inheritdoc/>
    public void Increment() {
      Increment(1);
    }

    /// <inheritdoc/>
    public void Increment(long n) {
      context_.Send(() => Update(n));
    }

    public void OnStep() {
      context_.Send(() => {
        prev_count_ = curr_count_;
        prev_tick_ = curr_tick_;
      });
    }

    double ConvertToUnit(double d) {
      return d/TimeUnit.Ticks.Convert(1, unit_);
    }

    /// <inheritdoc/>
    protected internal override Measure Compute(long tick) {
      curr_tick_ = tick;
      double delta = curr_tick_ - prev_tick_;
      double measure = (curr_count_ - prev_count_)/delta;
      return CreateMeasure(ConvertToUnit(measure));
    }

    void Update(long delta) {
      curr_count_ += delta;
      if (curr_count_ < 0) {
        curr_count_ = 0;
      }
    }
  }
}