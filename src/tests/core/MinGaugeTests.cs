using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class MinGaugeTests
  {
    [Test]
    public void should_retain_the_min_updated_value() {
      var gauge = new MinGauge(new MetricConfig("min1"));
      gauge.Update(42L);

      Measure measure = Testing.Sync<Measure>(gauge, gauge.GetMeasure, gauge.context_);
      Assert.That(measure.Value, Is.EqualTo(42.0));

      gauge = new MinGauge(new MetricConfig("min1"));
      gauge.Update(42L);
      gauge.Update(420L);

      measure = Testing.Sync<Measure>(gauge, gauge.GetMeasure, gauge.context_);
      Assert.That(measure.Value, Is.EqualTo(42.0));

      gauge = new MinGauge(new MetricConfig("min1"));
      gauge.Update(42L);
      gauge.Update(420L);
      gauge.Update(1L);

      measure = Testing.Sync<Measure>(gauge, gauge.GetMeasure, gauge.context_);
      Assert.That(measure.Value, Is.EqualTo(1.0));
    }

    [Test]
    public void should_report_zero_when_value_is_not_set() {
      var min = new MinGauge(new MetricConfig("min2"));

      Measure measure = Testing.Sync<Measure>(min, min.GetMeasure, min.context_);
      Assert.That(measure.Value, Is.EqualTo(0));
    }

    [Test]
    public void should_reset_the_gauge() {
      var min = new MinGauge(new MetricConfig("min2"));
      min.Update(42L);

      Measure measure = Testing.Sync<Measure>(min, min.GetMeasure, min.context_);
      Assert.That(measure.Value, Is.EqualTo(42L));

      min.Reset();
      min.Update(50L);
      measure = Testing.Sync<Measure>(min, min.GetMeasure, min.context_);
      Assert.That(measure.Value, Is.EqualTo(50D));
    }
  }
}
