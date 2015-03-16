using System;
using System.Threading;
using Nohros.Concurrent;
using Nohros.Metrics.Benchmarks;

namespace Nohros.Metrics.Tests.benchmarks
{
  public class CounterBenchmarks
  {
    long count_;
    readonly Random rand_;
    readonly Mailbox<Action> mailbox_;

    public CounterBenchmarks() {
      rand_ = new Random();
      mailbox_ = new Mailbox<Action>(runnable => runnable());
    }

    [Benchmark]
    [Category("Atomic")]
    public void Atomic() {
      Interlocked.Add(ref count_, rand_.Next());
    }

    [Benchmark]
    [Category("Mailbox")]
    public void Mailbox() {
      mailbox_.Send(() => {
        count_ += rand_.Next();
      });
    }
  }
}
