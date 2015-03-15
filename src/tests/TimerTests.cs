using System;
using System.Threading;
using NUnit.Framework;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  public class TimerTests
  {
    class ClockMock : Clock
    {
      long val_ = 0;

      public override long Tick {
        get { return val_ += 50000000; }
      }
    }

    [Test]
    public void ShouldCreateEmptyTimer() {
      var signaler = new ManualResetEvent(false);
      var timer = new Timer(new ClockMock());
      long count = 0;
      var snapshot = new Snapshot(new long[0]);

      timer.GetCount((x, context) => count = x);
      timer.GetSnapshot((x, timestamp) => snapshot = x);
      timer.Run(() => signaler.Set());

      signaler.WaitOne();

      Assert.That(count, Is.EqualTo(0));
      Assert.That(snapshot.Max, Is.InRange(0, 0.0001));
      Assert.That(snapshot.Min, Is.InRange(0, 0.0001));
      Assert.That(snapshot.Mean, Is.InRange(0, 0.0001));
      Assert.That(snapshot.StdDev, Is.InRange(0, 0.0001));
      Assert.That(snapshot.Median, Is.InRange(0, 0.0001));
      Assert.That(snapshot[0.75], Is.InRange(0, 0.0001));
      Assert.That(snapshot[0.99], Is.InRange(0, 0.0001));
      Assert.That(snapshot.Size, Is.EqualTo(0));
    }

    [Test]
    public void ShouldTimeSeriesOfEvents() {
      var signaler = new ManualResetEvent(false);
      var timer = new Timer();
      long count = 0;
      var snapshot = new Snapshot(new long[0]);

      timer.Update(10);
      timer.Update(20);
      timer.Update(20);
      timer.Update(30);
      timer.Update(40);

      timer.GetCount((x, context) => {
        count = x;
      });
      timer.GetSnapshot((x, timestamp) => snapshot = x);
      timer.Run(() => signaler.Set());

      signaler.WaitOne();

      Assert.That(count, Is.EqualTo(5));
      Assert.That(snapshot.Max, Is.InRange(40, 40.0001));
      Assert.That(snapshot.Min, Is.InRange(10, 10.0001));
      Assert.That(snapshot.Mean, Is.InRange(24, 24.0001));
      Assert.That(snapshot.StdDev, Is.InRange(11.401, 11.402));
      Assert.That(snapshot.Median, Is.InRange(20, 20.0001));
      Assert.That(snapshot[0.75], Is.InRange(35, 35.0001));
      Assert.That(snapshot[0.99], Is.InRange(40, 40.0001));
      Assert.That(snapshot.Values, Is.EqualTo(new double[] {10, 20, 20, 30, 40}));
      Assert.That(snapshot.Size, Is.EqualTo(5));
    }

    [Test]
    public void ShouldTimeThroughContext() {
      var signaler = new ManualResetEvent(false);
      var timer = new Timer();
      var snapshot = new Snapshot(new long[0]);
      long count = 0;
      timer.Time().Stop();

      timer.GetCount((x, context) => count = x);
      timer.Run(() => signaler.Set());

      signaler.WaitOne();

      Assert.That(count, Is.EqualTo(1), "the timer has count of 1");
    }
  }
}
