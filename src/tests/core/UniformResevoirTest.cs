using System;
using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class UniformResevoirTest
  {
    [Test]
    public void should_get_the_resevoir_size_when_recording_more_than_capacity() {
      var resevoir = new UniformResevoir(100);
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
      var resevoir = new UniformResevoir(100);
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
  }
}
