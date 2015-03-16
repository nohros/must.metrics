using System;
using System.Threading;
using Nohros.Concurrent;
using Nohros.Metrics.Benchmarks;

namespace Nohros.Metrics.Tests.Benchmarks
{
  [Category("Atomic")]
  public class InterlockedBenchmarks
  {
    long curr_count_;
    long curr_tick_;
    long prev_count_;
    long prev_tick_;
    double measure_;
    readonly object sync_;
    readonly Mailbox<Action> mailbox_;

    public InterlockedBenchmarks() {
      prev_count_ = 0;
      curr_count_ = 0;
      prev_tick_ = curr_tick_ = 10000;
      measure_ = 0;
      sync_= new object();
      mailbox_ = new Mailbox<Action>(runnable => runnable());
    }

    public void AtomicRead() {
      long curr_count = Interlocked.Read(ref curr_count_);
      long prev_tick = Interlocked.Read(ref prev_tick_);
      long prev_count = Interlocked.Read(ref prev_count_);
      Interlocked.Exchange(ref curr_tick_, 10000);
      double delta = curr_tick_ - prev_tick;
      double measure = (curr_count - prev_count) / delta;
      Interlocked.Exchange(ref measure_, measure);
    }

    public void ReadWithLock() {
      lock (sync_) {
        curr_tick_ = Clock.NanoTime;
        double delta = curr_tick_ - prev_tick_;
        measure_ = (curr_count_ - prev_count_) / (delta + 1);
      }
    }

    public void ReadWithMailbox() {
      long curr_count = curr_count_;
      mailbox_.Send(() => {
        curr_tick_ = Clock.NanoTime;
        double delta = curr_tick_ - prev_tick_;
        measure_ = (curr_count - prev_count_) / (delta + 1);
      });
    }

    public void Update (int delta) {
      Interlocked.Add(ref curr_count_, delta);
    }

    public void OnStep(int delta) {
      Interlocked.Exchange(ref prev_count_, curr_count_);
      Interlocked.Exchange(ref prev_tick_, curr_tick_);
    }
  }
}
