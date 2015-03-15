using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// An object which can produce statistical summaries.
  /// </summary>
  public interface ISummarizable
  {
    /// <summary>
    /// Gets the largest recorded value.
    /// </summary>
    /// <returns>The largest recorded value.</returns>
    double Max { get; }

    /// <summary>
    /// Gets the smallest recorded value.
    /// </summary>
    /// <returns>The smallest recorded value.</returns>
    double Min { get; }

    /// <summary>
    /// Gets the arithmetic mean of all recorded values.
    /// </summary>
    /// <returns>The arithmetic mean of all recorded values.</returns>
    double Mean { get; }

    /// <summary>
    /// Gets the standard deviation of all recorded values.
    /// </summary>
    /// <returns>The standard deviation of all recorded values.</returns>
    double StandardDeviation { get; }
  }
}
