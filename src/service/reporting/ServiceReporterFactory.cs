using System;
using System.Collections.Generic;
using Nohros.Concurrent;
using Nohros.Ruby;
using R = Nohros.Resources.StringResources;
using ZmqContext = ZMQ.Context;

namespace Nohros.Metrics.Reporting
{
  public class ServiceReporterFactory : IPollingMetricsReporterFactory
  {
    delegate IPollingMetricsReporter CreateServiceReporterDelegate(
      IDictionary<string, string> options);

    class Options
    {
      public string Server { get; set; }
      public IMetricsRegistry Registry { get; set; }
    }

    public const string kServerEndpoint = "endpoint";

    readonly CreateServiceReporterDelegate func_;
    readonly IMetricsRegistry registry_;
    readonly IRubyServiceHost service_host_;

    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="IMetricsReporterFactory"/>.
    /// </summary>
    public ServiceReporterFactory()
      : this((IMetricsRegistry) null) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="IMetricsReporterFactory"/>.
    /// </summary>
    public ServiceReporterFactory(IMetricsRegistry registry) {
      registry_ = registry;
      func_ = CreateServiceReporter;
    }

    public ServiceReporterFactory(IMetricsRegistry registry,
      IRubyServiceHost service_host) {
      if (service_host == null) {
        throw new ArgumentNullException("service_host");
      }
      registry_ = registry;
      service_host_ = service_host;
      func_ = CreateDynamicServiceReporter;
    }

    public ServiceReporterFactory(IRubyServiceHost service_host)
      : this(null, service_host) {
    }
    #endregion

    /// <inheritdoc/>
    public IPollingMetricsReporter CreatePollingReporter(
      IDictionary<string, string> options) {
      return func_(options);
    }

    IPollingMetricsReporter CreateServiceReporter(
      IDictionary<string, string> options) {
      var context = new ZmqContext();
      string server;
      if (!options.TryGetValue(kServerEndpoint, out server)) {
        throw new KeyNotFoundException(kServerEndpoint);
      }
      var registry = registry_ ??
        new AsyncMetricsRegistry(Executors.SameThreadExecutor());
      return new ServiceReporter(registry, context, server);
    }

    IPollingMetricsReporter CreateDynamicServiceReporter(
      IDictionary<string, string> options) {
      var registry = registry_ ??
        new AsyncMetricsRegistry(Executors.SameThreadExecutor());
      var context = new ZmqContext();
      return new DynamicServiceReporter(registry, context, service_host_);
    }
  }
}
