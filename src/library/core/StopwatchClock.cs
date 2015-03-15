using System;
using System.Diagnostics;

namespace Nohros.Metrics
{
  /// <summary>
  /// A <see cref="Clock"/> that uses a <see cref="Stopwatch"/> to mark the
  /// passage of time.
  /// </summary>
  public class StopwatchClock : Clock
  {
    readonly double tick_frequency_;

    public StopwatchClock() {
      tick_frequency_ = 10000000.0/Stopwatch.Frequency;
    }

    /// <inheritdoc/>
    public override long Tick {
      get { return (long) (Stopwatch.GetTimestamp()*tick_frequency_); }
    }
  }
}
