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
    BenchmarkOptions() {
      WarmUpTime = TimeSpan.FromSeconds(1);
      TestTime = TimeSpan.FromSeconds(10);
      Parallel = 1;
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

      int warmup = GetSwitchValueAsInt(cmd, "w", "warmup", 1);
      int duration = GetSwitchValueAsInt(cmd, "d", "duration", 10);
      int parallel = GetSwitchValueAsInt(cmd, "p", "parallel", 1);
      if (parallel <= 0) {
        parallel = 1;
      }
      string type = GetSwitchValue(cmd, "t", "type", null);
      string method = GetSwitchValue(cmd, "m", "method", null);

      bool raw = cmd.HasSwitch("r") || cmd.HasSwitch("raw");

      return new BenchmarkOptions {
        MethodFilter = method,
        TestTime = TimeSpan.FromSeconds(duration),
        WarmUpTime = TimeSpan.FromSeconds(warmup),
        Parallel = parallel,
        DisplayRawResults = raw
      };
    }

    static int GetSwitchValueAsInt(CommandLine cmd, string @switch,
      string abbrev, int @default) {
      return cmd
        .GetSwitchValueAsInt(abbrev, cmd.GetSwitchValueAsInt(@switch, @default));
    }

    static string GetSwitchValue(CommandLine cmd, string @switch,
      string abbrev, string @default) {
      return cmd.GetSwitchValue(abbrev, cmd.GetSwitchValue(@switch, @default));
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

    /// <summary>
    /// Gets the degree of paralellism to be used on test.
    /// </summary>
    /// <remarks>
    /// This option should be used only to test code that is thread-safe.
    /// Using it to test non thread-safe code could generate unexpected
    /// behavior.
    /// </remarks>
    public int Parallel { get; private set; }

    public bool DisplayRawResults { get; private set; }
  }
}
