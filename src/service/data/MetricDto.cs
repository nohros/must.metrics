using System;

namespace Nohros.Metrics.Data
{
  public class MetricDto
  {
    public string Name { get; set; }
    public long Timestamp { get; set; }
    public double Value { get; set; }
  }
}
