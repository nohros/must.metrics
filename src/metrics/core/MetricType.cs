using System;
using Nohros.Extensions;

namespace Nohros.Metrics
{
  /// <summary>
  /// A <see cref="Tag"/> that contain informations about a
  /// <see cref="MetricType"/>.
  /// </summary>
  public static class MetricTypeExtensions
  {
    const string kDefaultName = "type";

    /// <summary>
    /// Creates a <see cref="Tag"/> by using  the string "nohros.metrics.type"
    /// as the tag name and the given <paramref name="type"/> as the tag value.
    /// </summary>
    /// <param name="type">
    /// A <see cref="MetricType"/> that defines the value of the tag.
    /// </param>
    public static Tag AsTag(this MetricType type) {
      return AsTag(type, kDefaultName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricTypeExtensions"/> by using 
    /// the given <paramref name="name"/> as the tag name and the
    /// <paramref name="type"/> as the tag value.
    /// </summary>
    /// <param name="type">
    /// A <see cref="MetricType"/> that defines the value of the tag.
    /// </param>
    /// <param name="name">
    /// The name of the tag.
    /// </param>
    public static Tag AsTag(this MetricType type, string name) {
      switch (type) {
        case MetricType.Counter:
          return new Tag(name, "counter");
        case MetricType.Gauge:
          return new Tag(name, "gauge");
        case MetricType.EWMA:
          return new Tag(name, "ewma");
        case MetricType.Normalized:
          return new Tag(name, "normalized");
        default:
          throw new ArgumentOutOfRangeException(
            Resources.ArgIsInvalid.Fmt((int) type, typeof (MetricType).Name));
      }
    }
  }

  /// <summary>
  /// Indicates the type of a metric and determine how it will be measured.
  /// </summary>
  public enum MetricType
  {
    /// <summary>
    /// A gauge is for numeric values that can be sampled without modification.
    /// </summary>
    /// <remarks>
    /// Current temperature, number of open connections and disk usage are
    /// examples of metrics that should be gauges.
    /// </remarks>
    Gauge = 1,

    /// <summary>
    /// A counter is for numeric values that get incremented when some event
    /// occurs.
    /// </summary>
    /// <remarks>
    /// Counters could be sampled and converted into a rate of change
    /// per time unit.
    /// </remarks>
    Counter = 2,

    /// <summary>
    /// A normalized rate/unit. For counters that report values based on
    /// step boundaries.
    /// </summary>
    Normalized = 3,

    /// <summary>
    /// An exponentially-weighted moving average of count per time
    /// </summary>
    EWMA = 4,
  }
}
