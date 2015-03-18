using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class AppMetricsTests
  {
    public class MyClass
    {
      static readonly Counter counter_;

      static MyClass() {
        counter_ =
          new Counter(
            new MetricConfig("MyClass")
              .WithAdditionalTag("MyTag",
                "MyTagValue"));

        AppMetrics.RegisterObject(typeof (MyClass));
      }
    }

    [Test]
    public void should_register_static_metrics() {
      var @class = new MyClass();
      var metrics = new List<IMetric>();
      GetRawMetrics(AppMetrics.ForCurrentProcess.Metrics, metrics);
      bool contain = metrics.Any(m => m.Config.Tags.Any(t => t.Name == "MyTag"));
      Assert.That(contain, Is.True);
    }

    void GetRawMetrics(IEnumerable<IMetric> metrics, List<IMetric> raw) {
      foreach (var metric in metrics) {
        var composite = metric as ICompositeMetric;
        if (composite != null) {
          GetRawMetrics(composite.Metrics, raw);
        } else {
          raw.Add(metric);
        }
      }
    }
  }
}
