using System;
using System.Collections.Generic;
using Nohros.Ruby;
using ZMQ;
using ZmqContext = ZMQ.Context;
using ZmqSocket = ZMQ.Socket;

namespace Nohros.Metrics.Reporting
{
  /// <summary>
  /// A simple reporter which sends out application metrics to a metrics
  /// service periodically.
  /// </summary>
  public class ServiceReporter : AbstractPollingReporter
  {
    readonly ZmqContext context_;
    readonly string endpoint_;
    readonly ZmqSocket socket_;

    #region .ctor
    public ServiceReporter(IMetricsRegistry registry, ZmqContext context,
      string endpoint)
      : base(registry) {
      context_ = context;
      socket_ = context_.Socket(SocketType.DEALER);
      endpoint_ = "tcp://" + endpoint;
    }
    #endregion

    public override void Start(long period, TimeUnit unit) {
      socket_.Connect(endpoint_);
      base.Start(period, unit);
    }

    public override void Run() {
      var registry = MetricsRegistry;
      var now = DateTime.UtcNow;
      registry.Report(Report, now);
    }

    public override void Run(MetricPredicate predicate) {
      var registry = MetricsRegistry;
      var now = DateTime.UtcNow;
      registry.Report(Report, now, predicate);
    }

    void Report(KeyValuePair<string, MetricValue[]> metrics,
      DateTime timestamp) {
      MetricName name = metrics.Key;
      var protos = new StoreMetricsMessage.Builder();
      foreach (MetricValue metric in metrics.Value) {
        MetricProto proto = new MetricProto.Builder()
          .SetName(metric.Name)
          .SetValue(metric.Value)
          .SetTimestamp(TimeUnitHelper.ToUnixTime(timestamp.ToUniversalTime()))
          .SetUnit(metric.Unit)
          .Build();
        protos.AddMetrics(proto);
      }

      var packet = RubyMessages.CreateMessagePacket(new byte[0],
        (int) MessageType.kStoreMetricsMessage, protos.Build().ToByteArray());
      socket_.Send(packet.ToByteArray(), SendRecvOpt.NOBLOCK);
    }
  }
}
