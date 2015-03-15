using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Nohros.Metrics.Tests
{
  public class SnapshotTests
  {
    Snapshot snapshot_;

    public IResolveConstraint InRange(double value, double offset) {
      return Is.InRange(value - offset, value + offset);
    }

    [SetUp]
    public void SetUp() {
      snapshot_ = new Snapshot(new long[] {5, 1, 2, 3, 4});
    }

    [Test]
    public void small_quantiles_should_be_the_first_value() {
      Assert.That(snapshot_.Quantile(0.0), InRange(1, 0.1));
    }

    [Test]
    public void big_quantile_should_be_the_last_value() {
      Assert.That(snapshot_.Quantile(1.0), InRange(5, 0.1));
    }

    [Test]
    public void should_compute_the_median() {
      Assert.That(snapshot_.Median, InRange(3, 0.1));
    }

    [Test]
    public void should_compute_the_given_percentile() {
      Assert.That(snapshot_.Quantile(0.75), InRange(4.5, 0.1));
      Assert.That(snapshot_.Quantile(0.95), InRange(5, 0.1));
      Assert.That(snapshot_.Quantile(0.98), InRange(5, 0.1));
      Assert.That(snapshot_.Quantile(0.99), InRange(5, 0.1));
      Assert.That(snapshot_.Quantile(0.999), InRange(5, 0.1));
    }

    [Test]
    public void should_compute_the_minimum() {
      Assert.That(snapshot_.Min, Is.EqualTo(1));
    }

    [Test]
    public void should_compute_the_maximum() {
      Assert.That(snapshot_.Max, Is.EqualTo(5));
    }

    [Test]
    public void should_compute_the_mean() {
      Assert.That(snapshot_.Mean, Is.EqualTo(3.0));
    }

    [Test]
    public void should_compute_the_stdev() {
      Assert.That(snapshot_.StdDev, InRange(1.5811, 0.0001));
    }

    [Test]
    public void stats_should_be_zero_for_empty_snapshot() {
      var snapshot = new Snapshot(new long[] {});
      Assert.That(snapshot.Min, Is.EqualTo(0));
      Assert.That(snapshot.Max, Is.EqualTo(0));
      Assert.That(snapshot.Mean, Is.EqualTo(0));
      Assert.That(snapshot.StdDev, Is.EqualTo(0));
    }
  }
}
