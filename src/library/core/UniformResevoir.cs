using System;
using Nohros.Concurrent;

namespace Nohros.Metrics
{
  /// <summary>
  /// A ramdom sample of a stream of <see cref="long"/>'s. Uses Vitter's
  /// Algorithm R to produce a statically representative sample.
  /// </summary>
  /// <remarks>
  /// Ramdom Sampling with a Resovoir -
  ///   http://www.cs.umd.edu/~samir/498/vitter.pdf
  /// <para>
  /// <see cref="UniformResevoir"/> is not thread-safe.
  /// </para>
  /// </remarks>
  public class UniformResevoir : IResevoir
  {
    const int kBitsPerLong = 63;
    readonly Random rand_;
    readonly long[] resevoir_;
    readonly int resevoir_size_;
    readonly int resevoir_upper_limit_;
    int count_;

    /// <summary>
    /// Initializes a new instance of the <see cref="UniformResevoir"/> class
    /// that uses the specified executor to execute the update operation and
    /// keeps <paramref name="resevoir_size"/> elements in its resevoir.
    /// </summary>
    /// <param name="resevoir_size">
    /// The number of samples to keep in the sampling resevoir.
    /// </param>
    /// <remarks>
    /// The use of the executor returned by the method
    /// <see cref="Executors.SameThreadExecutor"/> is not encouraged, because
    /// the executor does not returns until the execution list is empty and,
    /// this can cause significant pauses in the thread that is executing the
    /// sample update.
    /// </remarks>
    public UniformResevoir(int resevoir_size) {
      resevoir_ = new long[resevoir_size];
      resevoir_size_ = resevoir_size;
      rand_ = new Random();
    }

    /// <inheritdoc/>
    public int Size {
      get {
        // The Size property is used less frequently than the Update method
        // so, it is better to keep the size logic here.
        return Math.Min(resevoir_size_, count_);
      }
    }

    /// <inheritdoc/>
    public Snapshot Snapshot {
      get {
        int size = Size;
        var resevoir = new long[size];
        Array.Copy(resevoir_, resevoir, size);
        return new Snapshot(resevoir);
      }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The update operation is performed asynchrounsly and is thread-safe.
    /// </remarks>
    public void Update(long value) {
      Update(value, 0);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// The update operation is performed asynchrounsly and is thread-safe.
    /// </remarks>
    public void Update(long value, long timestamp) {
      // resevoir_.length is always less thant [long.MaxValue], because the
      // resevoir size is an 32-bit integer. So, count never overflows,
      // because first condition.
      if (++count_ <= resevoir_size_) {
        resevoir_[count_ - 1] = value;
      } else {
        long r = NextLong(count_);
        if (r < resevoir_size_) {
          resevoir_[r] = value;
        }
      }
    }

    /// <inheritdoc/>
    public long Timestamp {
      get { return 0; }
    }

    /// <summary>
    /// Gets a pseudo-random long uniformly between 0 and n-1.
    /// </summary>
    /// <param name="n"></param>
    /// <returns>A value select randomly form the range <c>[0..n]</c></returns>
    long NextLong(long n) {
      long bits, val;
      do {
        long next_random_long = (long) (rand_.NextDouble()*2.0 - 1.0)*
          long.MaxValue;
        bits = next_random_long & (~(1L << kBitsPerLong));
        val = bits%n;
      } while (bits - val + (n - 1) < 0L);
      return val;
    }
  }
}
