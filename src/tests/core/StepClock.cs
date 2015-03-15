using System;

namespace Nohros.Metrics.Tests
{
  internal class StepClock : Clock
  {
    readonly long interval_;
    long tick_;

    public StepClock(TimeSpan interval) {
      interval_ = interval.Ticks;
      tick_ = 0;
    }

    public void TickNow(long count) {
      tick_ += interval_*count;
    }

    public override long Tick {
      get { return tick_; }
    }
  }
}
