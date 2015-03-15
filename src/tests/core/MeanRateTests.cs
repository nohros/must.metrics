using System;
using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class MeanRateTests
  {
    [Test]
    public void should_compute_the_mean_rate_of_values() {
      var clock = new StepClock(TimeSpan.FromMilliseconds(1));
      var mean = new MeanRate(new MetricConfig("test"), TimeUnit.Seconds,
        new MetricContext(clock));

      // add 1000 elemets at rate of 10 events/second
      for (int i = 0; i < 1000; i++) {
        mean.Mark();
        clock.TickNow(100);
      }

      double value =
        Testing.Sync<Measure>(mean, mean.GetMeasure, mean.context_).Value;
      Assert.That(value, Is.EqualTo(10));
    }
  }
}
