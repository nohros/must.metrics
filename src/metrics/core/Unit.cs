using System;
using Nohros;
using Nohros.Metrics;

namespace Nohros.Metrics
{
  public enum Unit
  {
    /// <summary>
    /// A nanosecond is defined as one thousandth of a microsecond.
    /// </summary>
    Nanoseconds = 0,

    /// <summary>
    /// A microsecond is defined as thousandth of a millisecond.
    /// </summary>
    Microseconds = 1,

    /// <summary>
    /// A millisecond is defined as thousandth of a second.
    /// </summary>
    Milliseconds = 2,

    /// <summary>
    /// The base unit of mensure.
    /// </summary>
    Seconds = 3,

    /// <summary>
    /// A minute is defined as sixty seconds.
    /// </summary>
    Minutes = 4,

    /// <summary>
    /// A hour is defined as sixty minutes.
    /// </summary>
    Hours = 5,

    /// <summary>
    /// A day is defined as twenty four hours.
    /// </summary>
    Days = 6,

    /// <summary>
    /// A percent is a ratio as a fraction of one hundred.
    /// </summary>
    Percent = 7,

    /// <summary>
    /// Quantites that has no unit.
    /// </summary>
    One = 100
  }

  public class UnitHelper
  {
    public static string FromTimeUnit(TimeUnit unit) {
      switch (unit) {
        case TimeUnit.Nanoseconds:
          return "ns";
        case TimeUnit.Microseconds:
          return "us";
        case TimeUnit.Milliseconds:
          return "ms";
        case TimeUnit.Seconds:
          return "s";
        case TimeUnit.Minutes:
          return "m";
        case TimeUnit.Hours:
          return "h";
        case TimeUnit.Days:
          return "d";
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public static string FromRate(string rate, TimeUnit unit) {
      return rate + "/" + FromTimeUnit(unit);
    }
  }
}
