using System;
using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class StepMaxGaugeTests
  {
    [Test]
    public void should_report_the_max_value_of_a_step() {
      var config = new MetricConfig("counter1");
      var max = new StepMaxGauge(config);

      max.Update(42L);
      max.GetMeasure(measure =>
        Assert.That(measure.Value, Is.EqualTo(42L)));

      max.OnStep();
      max.Update(10L);
      max.GetMeasure(measure =>
        Assert.That(measure.Value, Is.EqualTo(10L)));
    }
  }
}
