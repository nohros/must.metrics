// Copyright 2009 The Noda Time authors. All rights reserved.
// Copyright 2015 Nohros Inc. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;

namespace Nohros.Metrics.Benchmarks
{
  /// <summary>
  /// Encapsulates all the operations for benchmarking, such as the approximate
  /// length of each test, the timer to use and so on.
  /// </summary>
  public class BenchmarkOptions
  {
    private BenchmarkOptions() {
      WarmUpTime = TimeSpan.FromSeconds(1);
      TestTime = TimeSpan.FromSeconds(10);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BenchmarkOptions"/> class
    /// by using the process command argumants.
    /// </summary>
    /// <returns>
    /// An instanceo the <see cref="BenchmarkOptions"/> configured accordingly
    /// to the supplied command-line arguments.
    /// </returns>
    /// <see cref="Environment.CommandLine"/>
    /// <see cref="CommandLine"/>
    public static BenchmarkOptions FromCommandLine() {
      CommandLine cmd = CommandLine.ForCurrentProcess;

      // TODO (neylor.silva): Parse the command line and set the options.
      return new BenchmarkOptions();
    }

    /// <summary>
    /// Gets a <see cref="TimeSpan"/> representing the target per-test duration.
    /// </summary>
    /// <remarks>
    /// The default test time is 10 seconds.
    /// </remarks>
    public TimeSpan TestTime { get; private set; }

    /// <summary>
    /// Gets a <see cref="TimeSpan"/> representing the duration of the warm-up
    /// time per test.
    /// </summary>
    /// <remarks>
    /// The default warm-up time is 1 second.
    /// </remarks>
    public TimeSpan WarmUpTime { get; private set; }

    /// <summary>
    /// Gets a string containing the 
    /// </summary>
    public string TypeFilter { get; private set; }

    /// <summary>
    /// Gets a list of categories that should be included in the tests.
    /// </summary>
    /// <remarks>
    /// only the categories listed will be included in the test.
    /// <para>
    /// If the list is empty all the categories will be tested.
    /// </para>
    /// </remarks>
    public List<string> IncludedCategories { get; private set; }

    /// <summary>
    /// Gets a list of categories that should be excluded from the test.
    /// </summary>
    /// <remarks>
    /// If the list is empty no category will be excluded.
    /// </remarks>
    public List<string> ExcludedCategories { get; private set; }

    /// <summary>
    /// Gets a string taht defines the methods that should be tested.
    /// </summary>
    /// <remarks>
    /// The method filter performs the filter by using the method name. It
    /// accepts wildcards at the end of filter, i.e MyMethod*.s
    /// </remarks>
    public string MethodFilter { get; private set; }

    /// <summary>
    /// Gets flag that indicates if the test shoud run just one time and
    /// without timing, just to validate.
    /// </summary>
    public bool DryRunOnly { get; private set; }

    /// <summary>
    /// Gets the test label.
    /// </summary>
    public string Label { get; private set; }
  }
}