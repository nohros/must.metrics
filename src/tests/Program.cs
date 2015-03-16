using System;
using Nohros.Metrics.Benchmarks;

namespace Nohros.Metrics.Tests
{
  public class Program
  {
    public static int Main(string[] args) {
      var options = BenchmarkOptions.FromCommandLine();
      var handler = new ConsoleResultHandler(options.DisplayRawResults);
      var runner = new BenchmarkRunner(options, handler);
      runner.Run(typeof (Program).Assembly);
      return 1;
    }
  }
}
