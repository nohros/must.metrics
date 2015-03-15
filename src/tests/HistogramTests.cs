using System;
using NUnit.Framework;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  public class HistogramTests
  {
    [Test]
    public void ShouldCreateEmptyHistogram() {
      ISyncHistogram histogram = Histograms.Uniform();
      Assert.That(histogram.Count, Is.EqualTo(0));
      Assert.That(histogram.Max, Is.InRange(0, 0.0001));
      Assert.That(histogram.Min, Is.InRange(0, 0.0001));
      Assert.That(histogram.Mean, Is.InRange(0, 0.0001));
      Assert.That(histogram.StandardDeviation, Is.InRange(0, 0.0001));

      Snapshot snapshot = histogram.Snapshot;
      Assert.That(snapshot.Median, Is.InRange(0, 0.0001));
      Assert.That(snapshot.Percentile75, Is.InRange(0, 0.0001));
      Assert.That(snapshot.Percentile99, Is.InRange(0, 0.0001));
      Assert.That(snapshot.Size, Is.EqualTo(0));
    }

    [Test]
    public void ShouldCreateHistogramWith1000Elements() {
      ISyncHistogram histogram = Histograms.Uniform();

      for (int i = 1; i <= 1000; i++) {
        histogram.Update(i);
      }

      Assert.That(histogram.Count, Is.EqualTo(1000));
      Assert.That(histogram.Max, Is.InRange(999.9999, 1000.0001));
      Assert.That(histogram.Min, Is.InRange(0.9999, 1.0001));
      Assert.That(histogram.Mean, Is.InRange(500.4999, 500.50001));
      Assert.That(histogram.StandardDeviation, Is.InRange(288.8193, 288.8195));

      Snapshot snapshot = histogram.Snapshot;
      Assert.That(snapshot.Median, Is.InRange(500.4999, 500.50001));
      Assert.That(snapshot.Percentile75, Is.InRange(750.7499, 750.7501));
      Assert.That(snapshot.Percentile99, Is.InRange(990.9899, 990.9901));
      Assert.That(snapshot.Size, Is.EqualTo(1000));
    }
  }
}
