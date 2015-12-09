using System;
using System.Linq;
using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class TimerTests
  {
    [Test]
    public void should_measure_the_percentage_of_total_time_spent() {
      var config = new MetricConfig("timer1");
      var clock = new StepClock(TimeSpan.FromMinutes(1));
      var context = new MetricContext(clock);

      var timer = new Timer(config, TimeUnit.Seconds, context);

      IMetric total =
        timer
          .Metrics
          .First(
            x => x.Config.Tags.FirstOrDefault(t => t.Value == "total") != null);

      clock.TickNow(1);

      timer.Update(TimeSpan.FromSeconds(10));
      timer.Update(TimeSpan.FromSeconds(10));
      timer.Update(TimeSpan.FromSeconds(10));

      var measure = Measures.Get(total, context);
      Assert.That(measure.Value, Is.EqualTo(30d/60));

      clock.TickNow(1);
      timer.Update(TimeSpan.FromSeconds(10));

      measure = Measures.Get(total, context);
      Assert.That(measure.Value, Is.EqualTo(10d/60));
    }

    [Test]
    public void should_count_the_ratio_of_executions() {
      var config = new MetricConfig("timer1");
      var clock = new StepClock(TimeSpan.FromMinutes(1));
      var context = new MetricContext(clock);

      var timer = new Timer(config, TimeUnit.Seconds, context);

      IMetric count =
        timer
          .Metrics
          .First(
            x => x.Config.Tags.FirstOrDefault(t => t.Value == "count") != null);

      clock.TickNow(1);

      timer.Update(TimeSpan.FromSeconds(10));
      timer.Update(TimeSpan.FromSeconds(10));
      timer.Update(TimeSpan.FromSeconds(10));

      var measure = Measures.Get(count, context);
      Assert.That(measure.Value, Is.EqualTo(3d/60));

      clock.TickNow(1);
      timer.Update(TimeSpan.FromSeconds(10));

      measure = Measures.Get(count, context);
      Assert.That(measure.Value, Is.EqualTo(1d/60));
    }

    [Test]
    public void should_measure_the_maximum_value_per_interval() {
      var config = new MetricConfig("timer1");
      var clock = new StepClock(TimeSpan.FromMinutes(1));
      var context = new MetricContext(clock);

      var timer = new Timer(config, TimeUnit.Seconds, context);

      IMetric max =
        timer
          .Metrics
          .First(
            x => x.Config.Tags.FirstOrDefault(t => t.Value == "max") != null);

      clock.TickNow(1);

      timer.Update(TimeSpan.FromSeconds(10));
      timer.Update(TimeSpan.FromSeconds(30));
      timer.Update(TimeSpan.FromSeconds(10));

      var measure = Measures.Get(max, context);
      Assert.That(measure.Value, Is.EqualTo(30));

      clock.TickNow(1);
      timer.Update(TimeSpan.FromSeconds(5));

      measure = Measures.Get(max, context);
      Assert.That(measure.Value, Is.EqualTo(5));
    }

    [Test]
    public void should_measure_the_minimum_value_per_interval() {
      var config = new MetricConfig("timer1");
      var clock = new StepClock(TimeSpan.FromMinutes(1));
      var context = new MetricContext(clock);

      var timer = new Timer(config, TimeUnit.Seconds, context);

      IMetric min =
        timer
          .Metrics
          .First(
            x => x.Config.Tags.FirstOrDefault(t => t.Value == "min") != null);

      clock.TickNow(1);

      timer.Update(TimeSpan.FromSeconds(30));
      timer.Update(TimeSpan.FromSeconds(10));
      timer.Update(TimeSpan.FromSeconds(30));

      var measure = Measures.Get(min, context);
      Assert.That(measure.Value, Is.EqualTo(10));

      clock.TickNow(1);
      timer.Update(TimeSpan.FromSeconds(5));

      measure = Measures.Get(min, context);
      Assert.That(measure.Value, Is.EqualTo(5));
    }
  }
}
