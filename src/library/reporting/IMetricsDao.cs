using System;
using System.Collections.Generic;

namespace Nohros.Metrics.Reporting
{
  public interface IMetricsDao
  {
    IEnumerable<long> GetSeriesIds(string name, int hash, int tags_count);

    long RegisterSerie(string name, int hash, int count);
    void RegisterTag(string name, string value, long tags_id);
    bool ContainsTag(string name, string value, long tags_id);
    void RegisterMeasure(long tags_id, double value, DateTime timestamp);
  }
}
