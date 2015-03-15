using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A <see cref="IMetric"/> that provides the instantaneous values, for
  /// example, the percentage of disk space used or the size of a queue.
  /// </summary>
  public interface IGauge : IMetric
  {
  }
}
