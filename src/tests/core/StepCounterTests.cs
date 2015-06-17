using System;
using System.Threading;
using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class StepCounterTests
  {
    [Test]
    public void should_report_the_rate_of_change() {
      var minute = TimeSpan.FromMinutes(1);
      var config = new MetricConfig("counter1");
      var clock = new StepClock(TimeSpan.FromMinutes(1));
      var context = new MetricContext(clock);
      var counter = new StepCounter(config, TimeUnit.Minutes, context);
      
      clock.TickNow(1);
      counter.Increment(10);

      Measure measure = Testing.Sync<Measure>(counter, counter.GetMeasure,
        counter.context_);
      Assert.That(measure.Value, Is.EqualTo(10d));
      
      clock.TickNow(1);
      clock.TickNow(1);
      counter.Increment(10);

      measure = Testing.Sync<Measure>(counter, counter.GetMeasure,
        counter.context_, true);
      Assert.That(measure.Value, Is.EqualTo(5d),
        "Should report the same value as previously, since the rate was the same.");
      
      clock.TickNow(1);
      counter.Increment(20);

      measure = Testing.Sync<Measure>(counter, counter.GetMeasure,
        counter.context_);
      Assert.That(measure.Value, Is.EqualTo(20d),
        "Should report the double of the previously value, since the rate was doubled.");
    }
  }
}
