using System;
using System.Threading;
using Nohros.Concurrent;
using Nohros.Metrics.Benchmarks;

namespace Nohros.Metrics.Tests.Benchmarks
{
  public class MaxGaugeBenchmarks
  {
    readonly Mailbox<Action> mailbox_;
    long max_;
    readonly Random rand_;

    public MaxGaugeBenchmarks() {
      max_ = 0;
      rand_ = new Random();
      mailbox_ = new Mailbox<Action>(runnable => runnable());
    }

    [Benchmark]
    [Category("Atomic")]
    public void Atomic() {
      long value = Interlocked.Read(ref max_);
      long new_value = value + rand_.Next();
      while (Interlocked.CompareExchange(ref max_, new_value, value) != value) {
        value = Interlocked.Read(ref max_);
        new_value = value + 10;
      }
    }

    [Benchmark]
    [Category("Mailbox")]
    public void Mailbox() {
      mailbox_.Send(() => {
        long value = rand_.Next();
        if (value > max_) {
          max_ = value;
        }
      });
    }
  }
}
