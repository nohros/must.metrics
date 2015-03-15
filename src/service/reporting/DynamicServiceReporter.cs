using System;
using System.Collections.Generic;
using System.Threading;
using Nohros.Concurrent;
using Nohros.Metrics.Reporting;
using Nohros.Ruby;
using Nohros.Ruby.Protocol;
using Nohros.Ruby.Protocol.Control;
using R = Nohros.Resources.StringResources;
using ZmqContext = ZMQ.Context;

namespace Nohros.Metrics
{
  /// <summary>
  /// A implementation of the <see cref="IMetricsReporter"/> that uses the
  /// ruby infrastructure to discover the metrics service.
  /// </summary>
  public class DynamicServiceReporter : IPollingMetricsReporter,
                                        IRubyMessageListener
  {
    const string kClassName = "Nohros.Metrics.DynamicServiceReporter";

    readonly MetricsLogger logger_;
    readonly IMetricsRegistry registry_;
    readonly IRubyServiceHost service_host_;
    readonly ZmqContext context_;

    long period_;
    MetricPredicate predicate_;
    IPollingMetricsReporter reporter_;
    TimeUnit period_unit_;

    #region .ctor
    public DynamicServiceReporter(IMetricsRegistry registry,
      ZmqContext context, IRubyServiceHost service_host) {
      service_host_ = service_host;
      reporter_ = new NopMetricsReporter();
      period_ = 5;
      period_unit_ = TimeUnit.Seconds;
      registry_ = registry;
      logger_ = MetricsLogger.ForCurrentProcess;
      context_ = context;
    }
    #endregion

    /// <inheritdoc/>
    public void Start(long period, TimeUnit unit) {
      period_ = period;
      period_unit_ = unit;
      Start();
    }

    /// <inheritdoc/>
    public void Start(long period, TimeUnit unit, MetricPredicate predicate) {
      period_ = period;
      period_unit_ = unit;
      predicate_ = predicate;
      Start();
    }

    public void Shutdown(WaitHandle waiter) {
      if (reporter_ != null) {
        reporter_.Shutdown(waiter);
      }
    }

    public void Shutdown() {
      if (reporter_ != null) {
        reporter_.Shutdown();
      }
    }

    /// <inheritdoc/>
    void IRubyMessageListener.OnMessageReceived(IRubyMessage message) {
      switch (message.Type) {
        case (int) NodeMessageType.kNodeResponse:
          OnQueryResponse(message);
          break;
      }
    }

    void OnQueryResponse(IRubyMessage message) {
      try {
        ResponseMessage response = ResponseMessage.ParseFrom(message.Message);
        KeyValuePair endpoint;
        if (KeyValuePairs.Find(RubyStrings.kQueryResponseEndpointKey,
          response.ReponsesList, out endpoint)) {

          // create a new reporter using the informations of the found service.
          reporter_ = new ServiceReporter(registry_, context_, endpoint.Value);

          if (predicate_ != null) {
            reporter_.Start(period_, period_unit_);
          } else {
            reporter_.Start(period_, period_unit_, predicate_);
          }
        }
      } catch (Exception e) {
        logger_.Error(
          string.Format(R.Log_MethodThrowsException, "OnQueryResponse",
            kClassName), e);
      }
    }

    /// <inheritdoc/>
    void Start() {
      service_host_.AddListener(this, Executors.SameThreadExecutor());
    }
  }
}
