using Nohros.Metrics.Reporting;

namespace Nohros.Metrics
{
  /// <summary>
  /// Defines a metric that is mapped to a particular step interval.
  /// </summary>
  /// <remarks>
  /// The <see cref="IMetricsPoller"/> call the <see cref="OnStep"/>
  /// method of every registered metric that implements the
  /// <see cref="IStepMetric"/> class each time its
  /// <see cref="IMetricsPoller.Poll()"/> method is executed.
  /// </remarks>
  public interface IStepMetric : IMetric
  {
    /// <summary>
    /// The method that shoud be executed at every step interval.
    /// </summary>
    void OnStep();
  }
}