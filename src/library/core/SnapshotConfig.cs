using System;
using Nohros.Extensions;

namespace Nohros.Metrics
{
  /// <summary>
  /// Configuration options for metrics that compute values statistically
  /// through a <see cref="Snapshot"/>.
  /// </summary>
  /// <remarks>
  /// The default <see cref="SnapshotConfig"/> computes the count, min and max.
  /// </remarks>
  /// <seealso cref="Snapshot"/>
  /// <seealso cref="Histogram"/>
  public class SnapshotConfig
  {
    public class Builder
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="Builder"/> class that
      /// builds a <see cref="SnapshotConfig"/> that computes the mean,
      /// standard deviation, min, max and the the 95, 99 and 99.999 by
      /// percentiles by default.
      /// </summary>
      /// <param name="defaults">
      /// A flag indicating if the default values should be set.
      /// </param>
      public Builder(bool defaults = true) {
        if (defaults) {
          ComputeCount = true;
          ComputeMax = true;
          ComputeMin = true;
          Percentiles = new[] {95.0, 99.0};
        } else {
          Percentiles = new double[0];
        }
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="Builder"/> class by
      /// copying the configuration from the given <paramref name="config"/>.
      /// </summary>
      /// <param name="config"></param>
      public Builder(SnapshotConfig config) {
        Percentiles = config.Percentiles;
        ComputeCount = config.ComputeCount;
        ComputeMax = config.ComputeMax;
        ComputeMean = config.ComputeMean;
        ComputeMedian = config.ComputeMedian;
        ComputeMin = config.ComputeMin;
        ComputeStdDev = config.ComputeStdDev;
      }

      /// <summary>
      /// Defines that the count measure should be computed.
      /// </summary>
      public Builder WithCount(bool enable = true) {
        ComputeCount = enable;
        return this;
      }

      /// <summary>
      /// Defines that the median measure should be computed.
      /// </summary>
      public Builder WithMedian(bool enable = true) {
        ComputeMedian = enable;
        return this;
      }

      /// <summary>
      /// Defines that the max measure should be computed.
      /// </summary>
      public Builder WithMax(bool enable = true) {
        ComputeMax = enable;
        return this;
      }

      /// <summary>
      /// Defines that the min measure should be computed.
      /// </summary>
      public Builder WithMin(bool enable = true) {
        ComputeMin = enable;
        return this;
      }

      /// <summary>
      /// Defines that the mean measure should be computed.
      /// </summary>
      public Builder WithMean(bool enable = true) {
        ComputeMean = enable;
        return this;
      }

      /// <summary>
      /// Defines that the standard deviation measure should be computed.
      /// </summary>
      public Builder WithStdDev(bool enable = true) {
        ComputeStdDev = enable;
        return this;
      }

      /// <summary>
      /// Gets an array that indicates which percentiles should be computed.
      /// </summary>
      /// <remarks>
      /// The percentiles should be in the interval [0.0, 100.0].
      /// </remarks>
      public Builder WithPercentiles(double[] percentiles) {
        Percentiles = new double[percentiles.Length];
        for (int i = 0; i < percentiles.Length; i++) {
          double percentile = percentiles[i];
          if (percentile < 0.0 || percentile > 100.0) {
            throw new ArgumentOutOfRangeException("percentiles",
              Resources.ArgIsNotInRange.Fmt(0.0, 100.0));
          }
          Percentiles[i] = percentile;
        }
        return this;
      }

      /// <summary>
      /// Builds a <see cref="SnapshotConfig"/> using the configured
      /// definitions.
      /// </summary>
      public SnapshotConfig Build() {
        return new SnapshotConfig(this);
      }

      /// <summary>
      /// Gets a value indicating whether a count measure should be
      /// computed.
      /// </summary>
      public bool ComputeCount { get; private set; }

      /// <summary>
      /// Gets a value indicating whether a median measure should be
      /// computed.
      /// </summary>
      public bool ComputeMedian { get; private set; }

      /// <summary>
      /// Gets a value indicating whether a max measure should be
      /// computed.
      /// </summary>
      public bool ComputeMax { get; private set; }

      /// <summary>
      /// Gets a value indicating whether a min measure should be
      /// computed.
      /// </summary>
      public bool ComputeMin { get; private set; }

      /// <summary>
      /// Gets a value indicating whether a mean measure should be
      /// computed.
      /// </summary>
      public bool ComputeMean { get; private set; }

      /// <summary>
      /// Gets a value indicating whether a standard deviation measure should
      /// be computed.
      /// </summary>
      public bool ComputeStdDev { get; private set; }

      /// <summary>
      /// Gets an array that indicates which percentiles should be computed.
      /// </summary>
      /// <remarks>
      /// The percentiles should be in the interval [0.0, 100.0].
      /// </remarks>
      public double[] Percentiles { get; private set; }
    }

    SnapshotConfig(Builder builder) {
      ComputeCount = builder.ComputeCount;
      ComputeMax = builder.ComputeMax;
      ComputeMean = builder.ComputeMean;
      ComputeMedian = builder.ComputeMedian;
      ComputeMin = builder.ComputeMin;
      ComputeStdDev = builder.ComputeStdDev;

      Percentiles = new double[builder.Percentiles.Length];
      Array.Copy(builder.Percentiles, Percentiles, Percentiles.Length);
    }

    /// <summary>
    /// Gets a value indicating whether a count measure should be
    /// computed.
    /// </summary>
    public bool ComputeCount { get; private set; }

    /// <summary>
    /// Gets a value indicating whether a median measure should be
    /// computed.
    /// </summary>
    public bool ComputeMedian { get; private set; }

    /// <summary>
    /// Gets a value indicating whether a max measure should be
    /// computed.
    /// </summary>
    public bool ComputeMax { get; private set; }

    /// <summary>
    /// Gets a value indicating whether a min measure should be
    /// computed.
    /// </summary>
    public bool ComputeMin { get; private set; }

    /// <summary>
    /// Gets a value indicating whether a mean measure should be
    /// computed.
    /// </summary>
    public bool ComputeMean { get; private set; }

    /// <summary>
    /// Gets a value indicating whether a standard deviation measure should
    /// be computed.
    /// </summary>
    public bool ComputeStdDev { get; private set; }

    /// <summary>
    /// Gets an array that indicates which percentiles should be computed.
    /// </summary>
    /// <remarks>
    /// The percentiles should be in the interval [0.0, 100.0].
    /// </remarks>
    public double[] Percentiles { get; private set; }
  }
}
