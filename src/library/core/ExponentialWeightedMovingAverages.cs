using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  public partial class ExponentialWeightedMovingAverage
  {
    /// <summary>
    /// Creates a new <see cref="ExponentialWeightedMovingAverage"/> that
    /// computed a one minute load average and expectes to be ticked
    /// every 5 seconds.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <returns>
    /// A <see cref="ExponentialWeightedMovingAverage"/> which
    /// expectes to be ticked every 5 seconds.
    /// </returns>
    public static ExponentialWeightedMovingAverage ForOneMinute(
      MetricConfig config) {
      return new ExponentialWeightedMovingAverage(config, kOneMinuteAlpha,
        TimeSpan.FromSeconds(kFiveSecondsInterval));
    }

    /// <summary>
    /// Creates a new <see cref="ExponentialWeightedMovingAverage"/> that
    /// computed a five minute load average and expectes to be ticked
    /// every 5 seconds.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <returns>
    /// A <see cref="ExponentialWeightedMovingAverage"/> which
    /// expectes to be ticked every 5 seconds.
    /// </returns>
    public static ExponentialWeightedMovingAverage ForFiveMinutes(
      MetricConfig config) {
      return new ExponentialWeightedMovingAverage(config, kOneMinuteAlpha,
        TimeSpan.FromSeconds(kFiveSecondsInterval));
    }

    /// <summary>
    /// Creates a new <see cref="ExponentialWeightedMovingAverage"/> that
    /// computed a fifteen minute load average and expectes to be ticked
    /// every 5 seconds.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <returns>
    /// A <see cref="ExponentialWeightedMovingAverage"/> which
    /// expectes to be ticked every 5 seconds.
    /// </returns>
    public static ExponentialWeightedMovingAverage ForFifteenMinutes(
      MetricConfig config) {
      return new ExponentialWeightedMovingAverage(config, kOneMinuteAlpha,
        TimeSpan.FromSeconds(kFiveSecondsInterval));
    }

    /// <summary>
    /// Creates a new <see cref="ExponentialWeightedMovingAverage"/> that
    /// computed a one minute load average and expectes to be ticked
    /// every 5 seconds.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="unit">
    /// The time unit that should be used to compute the rate.
    /// </param>
    /// <param name="context">
    /// A <see cref="MetricContext"/> that contains the shared
    /// <see cref="Mailbox{T}"/> and <see cref="Clock"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ExponentialWeightedMovingAverage"/> which
    /// expectes to be ticked every 5 seconds.
    /// </returns>
    public static ExponentialWeightedMovingAverage ForOneMinute(
      MetricConfig config, TimeUnit unit, MetricContext context) {
      return new ExponentialWeightedMovingAverage(config, kOneMinuteAlpha,
        TimeSpan.FromSeconds(kFiveSecondsInterval), unit, context);
    }

    /// <summary>
    /// Creates a new <see cref="ExponentialWeightedMovingAverage"/> that
    /// computed a five minute load average and expectes to be ticked
    /// every 5 seconds.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="unit">
    /// The time unit that should be used to compute the rate.
    /// </param>
    /// <param name="context">
    /// A <see cref="MetricContext"/> that contains the shared
    /// <see cref="Mailbox{T}"/> and <see cref="Clock"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ExponentialWeightedMovingAverage"/> which
    /// expectes to be ticked every 5 seconds.
    /// </returns>
    public static ExponentialWeightedMovingAverage ForFiveMinutes(
      MetricConfig config, TimeUnit unit, MetricContext context) {
      return new ExponentialWeightedMovingAverage(config, kFiveMinutesAlpha,
        TimeSpan.FromSeconds(kFiveSecondsInterval), unit, context);
    }

    /// <summary>
    /// Creates a new <see cref="ExponentialWeightedMovingAverage"/> that
    /// computed a fifteen minute load average and expectes to be ticked
    /// every 5 seconds.
    /// </summary>
    /// <param name="config">
    /// A <see cref="MetricConfig"/> containing the configuration settings
    /// for the metric.
    /// </param>
    /// <param name="unit">
    /// The time unit that should be used to compute the rate.
    /// </param>
    /// <param name="context">
    /// A <see cref="MetricContext"/> that contains the shared
    /// <see cref="Mailbox{T}"/> and <see cref="Clock"/>.
    /// </param>
    /// <returns>
    /// A <see cref="ExponentialWeightedMovingAverage"/> which
    /// expectes to be ticked every 5 seconds.
    /// </returns>
    public static ExponentialWeightedMovingAverage ForFifteenMinutes(
      MetricConfig config, TimeUnit unit, MetricContext context) {
      return new ExponentialWeightedMovingAverage(config, kFifteenMinutesAlpha,
        TimeSpan.FromSeconds(kFiveSecondsInterval), unit, context);
    }
  }
}
