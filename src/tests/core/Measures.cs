namespace Nohros.Metrics.Tests
{
  public static class Measures
  {
    public static Measure Get(IMetric metric, MetricContext context) {
      var step = metric as IStepMetric;
      if (step == null) {
        return Testing.Sync<Measure>(metric, metric.GetMeasure, context);
      }
      return Testing.Sync<Measure>(step, step.GetMeasure, context, true);
    }
  }
}
