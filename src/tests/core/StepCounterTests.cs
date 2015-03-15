using System;
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
      var counter = new StepCounter(config, context);
      
      clock.TickNow(1);
      counter.Increment(10);

      Measure measure = Testing.Sync<Measure>(counter, counter.GetMeasure,
        counter.context_);
      Assert.That(measure.Value, Is.EqualTo(10d/minute.Ticks));
      
      counter.OnStep();
      clock.TickNow(1);
      counter.Increment(10);

      measure = Testing.Sync<Measure>(counter, counter.GetMeasure,
        counter.context_);
      Assert.That(measure.Value, Is.EqualTo(10d/minute.Ticks),
        "Should report the same value as previously, since the rate was the same.");
      
      counter.OnStep();
      clock.TickNow(1);
      counter.Increment(20);

      measure = Testing.Sync<Measure>(counter, counter.GetMeasure,
        counter.context_);
      Assert.That(measure.Value, Is.EqualTo(10d/minute.Ticks*2),
        "Should report the double of the previously value, since the rate was doubled.");
    }
  }
}
