// Copyright 2009 The Noda Time authors. All rights reserved.
// Copyright 2015 Nohros Inc. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;

namespace Nohros.Metrics.Benchmarks
{
  /// <summary>
  /// Attribute applied to any method which should be benchmarked.
  /// </summary>
  /// <remarks>
  /// The method must be parametersless, and its containing class must have a
  /// parameterless constructor. The constructor will be called just one,
  /// before all the tests are run - typically any initialization will just
  /// before readonly fields.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method)]
  public class BenchMarkAttribute : Attribute
  {
  }
}