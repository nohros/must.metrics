using System;
using System.Collections.Generic;
using System.Threading;
using Nohros.Metrics.Data;
using Nohros.Ruby;
using Nohros.Ruby.Protocol;
using S = Nohros.Resources.StringResources;

namespace Nohros.Metrics
{
  public class Service : AbstractRubyService
  {
    const string kClassName = "Nohros.Metrics.Service";

    readonly MetricsLogger logger_;
    readonly IMetricsDao metrics_dao_;
    readonly ISettings settings_;
    readonly ManualResetEvent start_stop_event_;
    IRubyServiceHost service_host_;

    #region .ctor
    public Service(ISettings settings, IMetricsDao metrics_dao)
    {
      settings_ = settings;
      metrics_dao_ = metrics_dao;
      logger_ = MetricsLogger.ForCurrentProcess;
      start_stop_event_ = new ManualResetEvent(false);
    }
    #endregion

    public override void Start(IRubyServiceHost service_host) {
      service_host_ = service_host;
      logger_.Logger = service_host_.Logger;
      start_stop_event_.WaitOne();
    }

    public override void Shutdown() {
      start_stop_event_.Set();
    }

    public override void OnMessage(IRubyMessage request) {
      try {
        switch (request.Type) {
          case (int) MessageType.kStoreMetricsMessage:
            var metrics = StoreMetricsMessage.ParseFrom(request.Message);
            StoreMetrics(metrics);
            break;
        }
      } catch (Exception e) {
        logger_.Error(string.Format(S.Log_MethodThrowsException, kClassName,
          "Store"), e);
      }
    }

    void StoreMetrics(StoreMetricsMessage request) {
      foreach (MetricProto proto in request.MetricsList) {
        metrics_dao_.Persist(new MetricDto {
          Name = proto.Name.Name,
          Timestamp = proto.Timestamp,
          Value = proto.Value
        });
      }
    }
  }
}
