using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Nohros.Metrics;

namespace Nohros.Toolkit.Metrics
{
  public class DottedMetricNameTests
  {
    [Test]
    public void should_concatenate_elements_eliding_null_and_empty_values() {
      string s = DottedMetricName.Format("first", null, "second");
      Assert.That(s, Is.EqualTo("first.second"));

      s = DottedMetricName.Format("first", "second", null);
      Assert.That(s, Is.EqualTo("first.second"));

      s = DottedMetricName.Format(null, "first", "second");
      Assert.That(s, Is.EqualTo("first.second"));

      s = DottedMetricName.Format("first", string.Empty, "second");
      Assert.That(s, Is.EqualTo("first.second"));

      s = DottedMetricName.Format("first", "second", string.Empty);
      Assert.That(s, Is.EqualTo("first.second"));

      s = DottedMetricName.Format(string.Empty, "first", "second");
      Assert.That(s, Is.EqualTo("first.second"));
    }
  }
}
