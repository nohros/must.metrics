using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nohros.Metrics.Tests
{
  public class ListClock : Clock
  {
    readonly Queue<long> queue_;

    public ListClock(IEnumerable<long> times) {
        queue_ = new Queue<long>(times);
    }

    public override long Tick {
      get { return queue_.Dequeue(); }
    }
  }
}
