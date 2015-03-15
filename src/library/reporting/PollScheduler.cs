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

    public PollScheduler(IMetricsPoller poller, TimeSpan interval) {
      poller_ = poller;
      scheduler_ = NonReentrantSchedule.Every(interval);
    }

    public void Start() {
      scheduler_.Run(poller_.Poll);
    }

    public WaitHandle Stop() {
      return scheduler_.Stop();
    }
  }
}
