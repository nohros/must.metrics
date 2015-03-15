using System;
using NUnit.Framework;
using Nohros.Concurrent;

namespace Nohros.Metrics.Tests
{
  public class ExponentialWeightedMovingAverageTest
  {
    static Mailbox<Action> mailbox_;
    static StepClock step_clock_;

    [SetUp]
    public void SetUp() {
      mailbox_ = new Mailbox<Action>(x => x());
      step_clock_ = new StepClock(TimeSpan.FromMilliseconds(5*1001));
    }

    [Test]
    public void should_decrease_the_average_at_each_minute() {
      var m1_ewma = ExponentialWeightedMovingAverage
        .ForOneMinute(new MetricConfig("test1"), TimeUnit.Seconds,
          new MetricContext(mailbox_, step_clock_));

      double offset = 0.000001;

      m1_ewma.Mark(3, step_clock_.Tick);

      step_clock_.TickNow(1);
      Assert.That(m1_ewma.Compute(step_clock_.Tick).Value,
        Is.InRange(0.6 - offset, 0.6 + offset));

      var averages = new[] {
        0.22072766,
        0.08120117,
        0.02987224,
        0.01098938,
        0.00404277,
        0.00148725,
        0.00054713,
        0.00020128,
        0.00007405,
        0.00002724,
        0.00001002,
        0.00000369,
        0.00000136,
        0.00000050,
        0.00000018
      };

      foreach (var average in averages) {
        step_clock_.TickNow(12); // 12* 5 seconds = 1 minute
        Assert.That(m1_ewma.Compute(step_clock_.Tick).Value,
          Is.InRange(average - offset, average + offset));
      }

      step_clock_.TickNow(10);
      Assert.That(m1_ewma.Compute(step_clock_.Tick).Value,
        Is.InRange(0.00000018 - offset, 0.00000018 + offset));
    }

    [Test]
    public void should_decrease_the_average_at_each_five_minutes() {
      var m1_ewma = ExponentialWeightedMovingAverage
        .ForFiveMinutes(new MetricConfig("test1"), TimeUnit.Seconds,
          new MetricContext(mailbox_, step_clock_));

      double offset = 0.000001;

      m1_ewma.Mark(3, step_clock_.Tick);

      step_clock_.TickNow(1);
      Assert.That(m1_ewma.Compute(step_clock_.Tick).Value,
        Is.InRange(0.6 - offset, 0.6 + offset));

      var averages = new[] {
        0.49123845,
        0.40219203,
        0.32928698,
        0.26959738,
        0.22072766,
        0.18071653,
        0.14795818,
        0.12113791,
        0.09917933,
        0.08120117,
        0.06648190,
        0.05443077,
        0.04456415,
        0.03648604,
        0.02987224
      };

      foreach (var average in averages) {
        step_clock_.TickNow(12); // +30 seconds
        Assert.That(m1_ewma.Compute(step_clock_.Tick).Value,
          Is.InRange(average - offset, average + offset));
      }

      step_clock_.TickNow(12);
    }

    [Test]
    public void should_decrease_the_average_at_each_fifteen_minutes() {
      var m1_ewma = ExponentialWeightedMovingAverage
        .ForFifteenMinutes(new MetricConfig("test1"), TimeUnit.Seconds,
          new MetricContext(mailbox_, step_clock_));

      double offset = 0.000001;

      m1_ewma.Mark(3, step_clock_.Tick);

      step_clock_.TickNow(1);
      Assert.That(m1_ewma.Compute(step_clock_.Tick).Value,
        Is.InRange(0.6 - offset, 0.6 + offset));

      var averages = new[] {
        0.56130419,
        0.52510399,
        0.49123845,
        0.45955700,
        0.42991879,
        0.40219203,
        0.37625345,
        0.35198773,
        0.32928698,
        0.30805027,
        0.28818318,
        0.26959738,
        0.25221023,
        0.23594443,
        0.22072766
      };

      foreach (var average in averages) {
        step_clock_.TickNow(12); // 12* 5 seconds = 1 minute
        Assert.That(m1_ewma.Compute(step_clock_.Tick).Value,
          Is.InRange(average - offset, average + offset));
      }

      step_clock_.TickNow(12);
    }
  }
}
