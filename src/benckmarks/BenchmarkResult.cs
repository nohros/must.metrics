// Copyright 2009 The Noda Time authors. All rights reserved.
// Copyright 2015 Nohros Inc. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Reflection;
using Nohros.Extensions.Time;

namespace Nohros.Metrics.Benchmarks
{
  /// <summary>
  /// Represents the results of running a single test.
  /// </summary>
  public class BenchmarkResult
  {
    readonly int iterations_;

    public BenchmarkResult(MethodInfo method, int iterations,
      TimeSpan duration) {
      Method = method;
      Duration = duration;
      iterations_ = iterations;
    }

    public MethodInfo Method { get; private set; }
    public TimeSpan Duration { get; private set; }

    public int Iterations { get; private set; }

    public long CallsPerSecond {
      get { return iterations_*TimeUnit.Seconds.ToTicks(1)/Duration.Ticks; }
    }

    public long NanosecondsPerCall {
      get { return (long) Duration.Convert(TimeUnit.Nanoseconds) / iterations_; }
    }
  }
}
