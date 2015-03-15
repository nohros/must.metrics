using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// A statistically representative sample of a data stream.
  /// </summary>
  public interface IResevoir
  {
    /// <summary>
    /// Gets the number of values recorded.
    /// </summary>
    /// <value>The number of values recorded.</value>
    int Size { get; }

    /// <summary>
    /// Adds a new recorded value to the sample.
    /// </summary>
    /// <param name="value">
    /// A new recorded value.
    /// </param>
    void Update(long value);

    /// <summary>
    /// Adds a new recorded value to the sample.
    /// </summary>
    /// <param name="value">
    /// A new recorded value.
    /// </param>
    /// <param name="timestamp">
    /// The time when the update was occurred.
    /// </param>
    /// <seealso cref="Timestamp"/>
    void Update(long value, long timestamp);

    /// <summary>
    /// Gets the current timestamp.
    /// </summary>
    long Timestamp { get; }

    /// <summary>
    /// Gets a snapshot of the sample's values.
    /// </summary>
    /// <value>A snapshot of the sample's values.</value>
    Snapshot Snapshot { get; }
  }
}
