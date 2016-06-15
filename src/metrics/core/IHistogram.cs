using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A metric which calculates the distribution of a value.
  /// </summary>
  /// <remarks>
  /// Accurately computing running variance
  /// <para>
  ///  http://www.johndcook.com/standard_deviation.html
  /// </para>
  /// </remarks>
  public interface IHistogram : IMetric
  {
    /// <summary>
    /// Adds a recorded value.
    /// </summary>
    void Update(long value);
  }
}
