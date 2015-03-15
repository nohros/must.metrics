using System;

namespace Nohros.Metrics.Benchmarks
{
  /// <summary>
  /// An implementation of the <see cref="BenchmarkResultHandler"/> that
  /// output benchmarks results to the console.
  /// </summary>
  public class ConsoleResultHandler : BenchmarkResultHandler
  {
    const string kLongFormatString =
      "  {0}: {1:N0} iterations/second; ({2:N0} iterations in {3:N0} ticks; {4:N0} nanoseconds/iteration)";

    const string kShortFormatString =
      "  {0}: {1:N0} iterations/second ({4:N0} nanoseconds/iteration)";

    readonly string format_string_;

    public ConsoleResultHandler(bool raw_results) {
      format_string_ = raw_results ? kLongFormatString : kShortFormatString;
    }

    public override void OnStartRun(BenchmarkOptions options) {
      Console.WriteLine("Environment: CLR {0} on {1} ({2})", Environment.Version,
        Environment.OSVersion,
        Environment.Is64BitProcess ? "64 bit" : "32 bit");
      if (options.Label != null) {
        Console.WriteLine("Run label: {0}", options.Label);
      }
    }

    public override void OnStartType(Type type) {
      Console.WriteLine("Running benchmarks in {0}", GetTypeDisplayName(type));
    }

    public override void OnResult(BenchmarkResult result) {
      Console
        .WriteLine(format_string_,
          result.Method.Name,
          result.CallsPerSecond,
          result.Iterations,
          result.Duration.Ticks,
          result.NanosecondsPerCall);
    }

    static string GetTypeDisplayName(Type type) {
      return type.FullName.Replace("Nohros.Metrics.Benchmarks.", "");
    }
  }
}
