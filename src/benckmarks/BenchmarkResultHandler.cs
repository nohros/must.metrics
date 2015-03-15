// Copyright 2009 The Noda Time authors. All rights reserved.
// Copyright 2015 Nohros Inc. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace Nohros.Metrics.Benchmarks
{
  /// <summary>
  /// Handler for benchmark results.
  /// </summary>
  /// <remarks>
  /// While we could have used events on BenchmarkRunner, it's likely that a
  /// bunch of these will be customized at the same time, so it makes more
  /// sense to make it a normal class.
  /// </remarks>
  public class BenchmarkResultHandler
  {
    /// <summary>
    /// Method that is called at the very start of the set of tests.
    /// </summary>
    /// <param name="options">
    /// A <see cref="BenchmarkOptions"/> contianing the options used in the
    /// associated test.
    /// </param>
    public virtual void OnStartRun(BenchmarkOptions options) {
    }

    /// <summary>
    /// Method that is called at the very end of the set of tests.
    /// </summary>
    public virtual void OnEndRun() {
    }

    /// <summary>
    /// Method that is called at the start of benchmarks for a single type.
    /// </summary>
    public virtual void OnStartType(Type type) {
    }

    /// <summary>
    /// Method that is called at the end of benchmarks for a single type.
    /// </summary>
    public virtual void OnEndType() {
    }

    /// <summary>
    /// Method that is called once for each test.
    /// </summary>
    public virtual void OnResult(BenchmarkResult result) {
    }

    /// <summary>
    /// Method that is called each time a type or method isn't tested
    /// unexpectedly.
    /// </summary>
    /// <param name="text">
    /// The reason why the type or method isn't tested unexpectedly.
    /// </param>
    public virtual void OnWarning(string text) {
    }
  }
}
