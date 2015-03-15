using System;
using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class CounterTest
  {
    [Test]
    public void should_start_counting_at_zero() {
      var counter = new Counter(new MetricConfig("counter1"));
      double count = Testing.Sync(counter, counter.GetMeasure, counter.context_);
      Assert.That(count, Is.EqualTo(0));
    }

    [Test]
    public void should_increment_counter_by_one() {
      var counter = new Counter(new MetricConfig("counter1"), 9);
      counter.Increment();

      double count = Testing.Sync(counter, counter.GetMeasure, counter.context_);

      Assert.That(count, Is.EqualTo(10));
    }

    [Test]
    public void should_increment_counter_by_given_delta() {
      var counter = new Counter(new MetricConfig("counter1"));
      counter.Increment(15);

      double count = Testing.Sync(counter, counter.GetMeasure, counter.context_);
      Assert.That(count, Is.EqualTo(15));
    }

    [Test]
    public void should_decrement_counter_by_one() {
      var counter = new Counter(new MetricConfig("counter1"), 10);
      counter.Decrement();
      double count = Testing.Sync(counter, counter.GetMeasure, counter.context_);
      Assert.That(count, Is.EqualTo(10 - 1));
    }

    [Test]
    public void should_decrement_counter_by_given_delta() {
      var counter = new Counter(new MetricConfig("counter1"), 15);
      counter.Decrement(12);
      double count = Testing.Sync(counter, counter.GetMeasure, counter.context_);
      Assert.That(count, Is.EqualTo(15 - 12));
    }
  }
}
