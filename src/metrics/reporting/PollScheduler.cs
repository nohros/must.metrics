using System;
using System.Threading;
using Nohros.Concurrent;

namespace Nohros.Metrics.Reporting
{
  /// <summary>
  /// A basic scheduler for polling metrics that fetch metrics using a
  /// dedicated background thread.
  /// </summary>
  public class PollScheduler
  {
    readonly IMetricsPoller poller_;
    readonly NonReentrantSchedule scheduler_;

    /// <summary>
    /// Initializes a new instance of the <see cref="PollScheduler"/> by
    /// using the given <paramref name="poller"/> and
    /// <paramref name="interval"/>.
    /// </summary>
    /// <param name="poller">
    /// A <see cref="IMetricsPoller"/> that can be used to poll metrics for
    /// measures.
    /// </param>
    /// <param name="interval">
    /// The interval that should be used to poll metrics for its measures.
    /// </param>
    public PollScheduler(IMetricsPoller poller, TimeSpan interval) {
      poller_ = poller;
      scheduler_ = NonReentrantSchedule.Every(interval);
    }

    /// <summary>
    /// Start the metrics poller.
    /// </summary>
    public void Start() {
      scheduler_.Run(poller_.Poll);
    }

    /// <summary>
    /// Stops the metrics poller and returns an <see cref="WaitHandle"/> that
    /// can be used to wait monitor the status of the stop operation.
    /// </summary>
    /// <returns>
    /// A <see cref="WaitHandle"/> that can be used to monitor the status of
    /// the stop operation.
    /// </returns>
    /// <remarks>
    /// The stop operation is performed asynchronously. You should use the
    /// returned <see cref="WaitHandle"/> to check for the poller stop
    /// operation status.
    /// </remarks>
    public WaitHandle Stop() {
      return scheduler_.Stop();
    }
  }
}
