using System;
using Nohros.Logging;

namespace Nohros.Metrics
{
  public class MetricsLogger : ForwardingLogger
  {
    static MetricsLogger() {
      ForCurrentProcess = new MetricsLogger(new NOPLogger());
    }

    public MetricsLogger(ILogger logger) : base(logger) {
    }

    public static MetricsLogger ForCurrentProcess { get; set; }
  }
}
