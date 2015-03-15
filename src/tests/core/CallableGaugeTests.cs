using System;
using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class CallableGaugeTests
  {
    [Test]
    public void should_compute_measure_using_func() {
      var gauge = new CallableGauge(new MetricConfig("test"), () => 10);
      double value =
        Testing.Sync<Measure>(gauge, gauge.GetMeasure, gauge.context_).Value;
      Assert.That(value, Is.EqualTo(10));
    }
  }
}
