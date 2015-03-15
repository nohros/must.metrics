using System;
using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class StepMinGaugeTests
  {
    [Test]
    public void should_report_the_max_value_of_a_step()
    {
      var config = new MetricConfig("counter1");
      var min = new StepMaxGauge(config);

      min.Update(42L);
      min.GetMeasure(measure =>
        Assert.That(measure.Value, Is.EqualTo(42L)));

      min.OnStep();
      min.Update(50L);
      min.GetMeasure(measure =>
        Assert.That(measure.Value, Is.EqualTo(50L)));
    }
  }
}
