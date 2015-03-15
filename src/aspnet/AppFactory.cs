using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  public class AppFactory
  {
    readonly ISettings settings_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="AppFactory"/> using the
    /// specified application settings.
    /// </summary>
    /// <param name="settings">
    /// A <see cref="ISettings"/> object containig the user configured settings.
    /// </param>
    public AppFactory(ISettings settings) {
      settings_ = settings;
    }
    #endregion

    public MetricsReporter CreateMetricsApplication() {
      var registry = new AsyncMetricsRegistry(Executors.ThreadPoolExecutor());
      return new MetricsReporter(settings_, registry);
    }
  }
}
