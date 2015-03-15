using System;
using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class ExponentiallyDecayingReservoirTests
  {
    [Test]
    public void should_get_the_resevoir_size_when_recording_more_than_capacity() {
      var resevoir = new ExponentiallyDecayingResevoir(100, 0.99);
      for (int i = 0; i < 1000; i++) {
        resevoir.Update(i);
      }

      Snapshot snapshot = resevoir.Snapshot;
      Assert.That(resevoir.Size, Is.EqualTo(100),
        "Resevoir size is not equals to defined");
      Assert.That(snapshot.Size, Is.EqualTo(100),
        "Snapshot size is different from the resevoit size");

      foreach (var value in snapshot.Values) {
        Assert.That(value, Is.LessThan(1000).And.GreaterThanOrEqualTo(0));
      }
    }

    [Test]
    public void should_get_the_recorded_size_when_recording_less_than_capacity() {
      var resevoir = new ExponentiallyDecayingResevoir(100, 0.99);
      for (int i = 0; i < 99; i++) {
        resevoir.Update(i);
      }

      Snapshot snapshot = resevoir.Snapshot;
      Assert.That(resevoir.Size, Is.EqualTo(99));
      Assert.That(snapshot.Size, Is.EqualTo(99));

      foreach (var value in snapshot.Values) {
        Assert.That(value, Is.LessThan(1000).And.GreaterThanOrEqualTo(0));
      }
    }

    [Test]
    public void long_period_of_inactivity_should_not_corrupt_sampling_state() {
      var clock = new StepClock(TimeSpan.FromMilliseconds(1));
      var resevoir = new ExponentiallyDecayingResevoir(10, 0.015, clock);

      // add 1000 values at a rate of 10 values/second.
      for (int i = 0; i < 1000; i++) {
        resevoir.Update(1000 + i);
        clock.TickNow(100);
      }

      AssertAllValuesBetween(resevoir, 1000, 2000);

      // wait for 15 hours and add another value.
      // this should trigger a rescale. Note that the number of samples will be
      // reduced to 2 because of the  very small factor that will make all
      // existing priorities equal to zero after rescale.
      clock.TickNow(15*60*60*1000);
      resevoir.Update(2000);
      Assert.That(resevoir.Snapshot.Size, Is.EqualTo(2));
      AssertAllValuesBetween(resevoir, 1000, 3000);

      // add 1000 values at a rate of 10 values/second.
      for (int i = 0; i < 1000; i++) {
        resevoir.Update(3000 + i);
        clock.TickNow(100);
      }

      Assert.That(resevoir.Snapshot.Size, Is.EqualTo(10));
      AssertAllValuesBetween(resevoir, 3000, 4000);
    }

    public void AssertAllValuesBetween(ExponentiallyDecayingResevoir resevoir,
      long min, long max) {
      Snapshot snapshot = resevoir.Snapshot;
      foreach (var value in snapshot.Values) {
        Assert.That(value, Is.InRange(min, max));
      }
    }
  }
}
