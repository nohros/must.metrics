using System;
using NUnit.Framework;
using Nohros.Extensions.Time;
using Telerik.JustMock;
using Nohros.Extensions;

namespace Nohros.Metrics.Tests
{
  public class MeterTests
  {
    [SetUp]
    public void SetUp() {
    }

    [Test]
    public void ShouldCreateBlankMeter() {
      var meter = new Meter("thing", TimeUnit.Seconds);
      Assert.That(meter.Count, Is.EqualTo(0));
    }

    [Test]
    public void ShouldCreateMeterWithThreeeEvents() {
      var meter = new Meter("thing", TimeUnit.Seconds);
      meter.Mark(3);
      Assert.That(meter.Count, Is.EqualTo(3));
    }

    [Test]
    public void should_mark_evets_and_update_rates_count() {
      var clock = new ListClock(new[] {
        0L, 0L, 10L.ToNanoseconds(TimeUnit.Seconds), 0L, 0L, 0L
      });
      var meter = new Meter(clock);
      meter.Mark();
      meter.Mark(2);
      Assert.That(meter.OneMinuteRate, Is.InRange(0.1830, 0.1850));
      Assert.That(meter.FiveMinuteRate, Is.InRange(0.1966, 0.1976));
      Assert.That(meter.FifteenMinuteRate, Is.InRange(0.1988, 0.1998));
    }
  }
}
