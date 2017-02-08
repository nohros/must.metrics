using System;
using System.Collections.Generic;
using System.Linq;
using Nohros.Extensions;

namespace Nohros.Metrics
{
  /// <summary>
  /// A statistical snapshot of a <see cref="Snapshot"/>.
  /// </summary>
  public class Snapshot
  {
    /// <summary>
    /// The quantile for median.
    /// </summary>
    public const double kMedianQuantile = 0.5;

    /// <summary>
    /// The quantile to be used to get the value that fall below 75% of the
    /// observations.
    /// </summary>
    public const double k75Percentile = 0.75;

    /// <summary>
    /// The quantile to be used to get the value that fall below 95% of the
    /// observations.
    /// </summary>
    public const double k95Percentile = 0.95;

    /// <summary>
    /// The quantile to be used to get the value that fall below 98% of the
    /// observations.
    /// </summary>
    public const double k98Percentile = 0.98;

    /// <summary>
    /// The quantile to be used to get the value that fall below 99% of the
    /// observations.
    /// </summary>
    public const double k99Percentile = 0.99;

    /// <summary>
    /// The quantile to be used to get the value that fall below 99,9% of the
    /// observations.
    /// </summary>
    public const double k999Percentile = 0.999;

    readonly long[] values_;

    /// <summary>
    /// Initializes a new instance of the <see cref="Snapshot"/> class by using
    /// the specifid collection of long values.
    /// </summary>
    /// <param name="values">An unordered set of values in the resevoir.</param>
    public Snapshot(IEnumerable<long> values) {
      values_ = values.OrderBy(v => v).ToArray();
    }

    /// <summary>
    /// Gets the value at the given quantile.
    /// </summary>
    /// <param name="quantile">A quantile, in <c>[0...1]</c></param>
    /// <returns>The value in the distribution at <paramref name="quantile"/>.
    /// </returns>
    public double Quantile(double quantile) {
      if (quantile < 0.0 || quantile > 1.0) {
        throw new ArgumentOutOfRangeException("quantile",
          Resources.ArgIsNotInRange.Fmt(0, 1));
      }

      if (values_.Length == 0) {
        return 0.0;
      }

      double pos = quantile*(values_.Length + 1);
      if (pos < 1) {
        return values_[0];
      }

      if (pos >= values_.Length) {
        return values_[values_.Length - 1];
      }

      double lower = values_[(int) pos - 1];
      double upper = values_[(int) pos];
      return lower + (pos - Math.Floor(pos))*(upper - lower);
    }

    /// <summary>
    /// Gets the value at the given quantile.
    /// </summary>
    /// <param name="quantile">A quantile, in <c>[0...1]</c></param>
    /// <returns>The value in the distribution at <paramref name="quantile"/>.
    /// </returns>
    public double this[double quantile] {
      get { return Quantile(quantile); }
    }

    /// <summary>
    /// Gets the number of items in the snapshot.
    /// </summary>
    public int Size {
      get { return values_.Length; }
    }

    /// <summary>
    /// Gets hte median value in the distribution.
    /// </summary>
    public double Median {
      get { return this[kMedianQuantile]; }
    }

    /// <summary>
    /// Gets the highest value is the snapshot.
    /// </summary>
    public long Max {
      get {
        if (values_.Length == 0) {
          return 0;
        }
        return values_[values_.Length - 1];
      }
    }

    /// <summary>
    /// Gets the lowest value is the snapshot.
    /// </summary>
    public long Min {
      get {
        if (values_.Length == 0) {
          return 0;
        }
        return values_[0];
      }
    }

    /// <summary>
    /// Gets teh arithmetic mean of the values in the snapshot.
    /// </summary>
    public double Mean {
      get {
        if (values_.Length == 0) {
          return 0;
        }

        double sum = 0;
        for (int i = 0; i < values_.Length; i++) {
          sum += values_[i];
        }
        return sum/values_.Length;
      }
    }

    /// <summary>
    /// Gets the standard deviation of the values in the snapshot.
    /// </summary>
    public double StdDev {
      get {
        // two-pass algorithm for variance, avoids numeric overflow
        if (values_.Length <= 1) {
          return 0;
        }

        double mean = Mean;
        double sum = 0;

        for (int i = 0; i < values_.Length; i++) {
          double diff = values_[i] - mean;
          sum += diff*diff;
        }

        double variance = sum/(values_.Length - 1);
        return Math.Sqrt(variance);
      }
    }

    /// <summary>
    /// Get the entire set of values in the snapshot.
    /// </summary>
    /// <value>
    /// The entire set of values in snapshot.
    /// </value>
    public long[] Values {
      get {
        int length = values_.Length;
        long[] copy = new long[length];
        values_.CopyTo(copy, 0);
        return copy;
      }
    }
  }
}
