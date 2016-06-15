using System;
using System.Collections.Generic;
using System.Text;

namespace Nohros.Metrics
{
  /// <summary>
  /// Interface for <see cref="IMetric"/> instances that can be stopped.
  /// </summary>
  public interface IStoppable
  {
    /// <summary>
    /// Stop the instance.
    /// </summary>
    void Stop();
  }
}
