using System;
using Nohros.Logging;

namespace Nohros.Metrics.Reporting
{
  public class MetricsLogger : ForwardingLogger
  {
    static readonly MetricsLogger current_process_logger_;

    #region .ctor
    static MetricsLogger() {
      current_process_logger_ = new MetricsLogger(new NOPLogger());
    }
    #endregion

    #region .ctor
    public MetricsLogger(ILogger logger) : base(logger) {
    }
    #endregion

    public static MetricsLogger ForCurrentProcess {
      get { return current_process_logger_; }
    }
  }
}
