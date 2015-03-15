using System;
using System.Threading;

namespace Nohros.Metrics.Tests
{
  public static class Testing
  {
    public static T Sync<T>(IMetric metric, Action<Action<T>> async,
      MetricContext context) {
      var signaler = new ManualResetEventSlim(false);
      T result = default(T);
      async(arg1 => result = arg1);
      context.Send(signaler.Set);
      signaler.Wait();
      return result;
    }

    public static double Sync(IMetric metric, Action<Action<Measure>> async,
      MetricContext context) {
      return Sync<Measure>(metric, async, context).Value;
    }
  }
}
