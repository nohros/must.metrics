using System;
using System.Threading;
using NUnit.Framework;
using System.Linq;

namespace Nohros.Metrics.Tests
{
  public class BucketTimerTests
  {
    [Test]
    public void should_measure_the_total_time_per_step() {
      var minute = TimeSpan.FromMinutes(1);
      var config = new MetricConfig("counter1");
      var clock = new StepClock(TimeSpan.FromMinutes(1));
      var context = new MetricContext(clock);
      var timer =
        new BucketTimer.Builder(config)
          .WithBuckets(new long[] {60, 120, 180})
          .WithTimeUnit(TimeUnit.Seconds)
          .WithRateUnit(TimeUnit.Minutes)
          .WithContext(context)
          .Build();

      IMetric total =
        timer
          .Metrics
          .First(
            x => x.Config.Tags.FirstOrDefault(t => t.Value == "total.time") != null);

      clock.TickNow(1);

      timer.Update(TimeSpan.FromSeconds(10));
      timer.Update(TimeSpan.FromSeconds(10));
      timer.Update(TimeSpan.FromSeconds(10));

      var measure = GetMeasure(total, context);
      Assert.That(measure.Value, Is.EqualTo(30d/60));

      clock.TickNow(1);

      timer.Update(TimeSpan.FromSeconds(30));

      measure = GetMeasure(total, context);
      Assert.That(measure.Value, Is.EqualTo(30d/60));

      clock.TickNow(1);

      timer.Update(TimeSpan.FromSeconds(60));

      measure = Testing.Sync<Measure>(total, total.GetMeasure, context);
      Assert.That(measure.Value, Is.EqualTo(1));
    }

    [Test]
    public void should_count_the_number_of_times_a_bucket_was_hit() {
      var minute = TimeSpan.FromMinutes(1);
      var config = new MetricConfig("counter1");
      var clock = new StepClock(TimeSpan.FromMinutes(3));
      var context = new MetricContext(clock);
      var timer =
        new BucketTimer.Builder(config)
          .WithBuckets(new long[] {60, 120, 180})
          .WithTimeUnit(TimeUnit.Seconds)
          .WithRateUnit(TimeUnit.Minutes)
          .WithContext(context)
          .Build();

      IMetric b60 = GetMetricWithTag(timer, "bucket=060s");
      IMetric b120 = GetMetricWithTag(timer, "bucket=120s");
      IMetric b180 = GetMetricWithTag(timer, "bucket=180s");

      clock.TickNow(1);

      timer.Update(TimeSpan.FromSeconds(10));
      timer.Update(TimeSpan.FromSeconds(10));
      timer.Update(TimeSpan.FromSeconds(10));

      var measure = GetMeasure(b60, context);
      Assert.That(measure.Value, Is.EqualTo(1));

      measure = GetMeasure(b120, context);
      Assert.That(measure.Value, Is.EqualTo(0));

      measure = GetMeasure(b180, context);
      Assert.That(measure.Value, Is.EqualTo(0));

      clock.TickNow(1);

      timer.Update(TimeSpan.FromSeconds(30));
      timer.Update(TimeSpan.FromSeconds(61));
      timer.Update(TimeSpan.FromSeconds(65));

      measure = GetMeasure(b60, context);
      Assert.That(measure.Value, Is.EqualTo(1/3d));

      measure = GetMeasure(b120, context);
      Assert.That(measure.Value, Is.EqualTo(2/3d));

      measure = GetMeasure(b180, context);
      Assert.That(measure.Value, Is.EqualTo(0));

      clock.TickNow(1);

      timer.Update(TimeSpan.FromSeconds(180));

      measure = GetMeasure(b60, context);
      Assert.That(measure.Value, Is.EqualTo(0));

      measure = GetMeasure(b120, context);
      Assert.That(measure.Value, Is.EqualTo(0));

      measure = GetMeasure(b180, context);
      Assert.That(measure.Value, Is.EqualTo(1/3d));
    }

    Measure GetMeasure(IMetric metric, MetricContext context) {
      var step = metric as IStepMetric;
      if (step == null) {
        return Testing.Sync<Measure>(metric, metric.GetMeasure, context);
      }
      return Testing.Sync<Measure>(step, step.GetMeasure, context, true);
    }

    IMetric GetMetricWithTag(ICompositeMetric metric, string value) {
      return
        metric
          .Metrics
          .First(
            x =>
              x.Config.Tags.FirstOrDefault(t => t.Value == value) != null);
    }
  }
}
