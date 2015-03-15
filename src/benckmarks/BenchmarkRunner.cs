using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Nohros.Extensions;

namespace Nohros.Metrics.Benchmarks
{
  public sealed class BenchmarkRunner
  {
    const BindingFlags kAllInstance =
      BindingFlags.Public |
        BindingFlags.NonPublic |
        BindingFlags.Instance;

    readonly BenchmarkOptions options_;
    readonly BenchmarkResultHandler result_handler_;

    public BenchmarkRunner(BenchmarkOptions options,
      BenchmarkResultHandler handler) {
      options_ = options;
      result_handler_ = handler;
    }

    /// <summary>
    /// Run the tests for all types and methods annotated with the
    /// <see cref="BenchMarkAttribute"/> attribute in the specified
    /// <paramref name="assembly"/>.
    /// </summary>
    /// <param name="assembly">
    /// The assembly containing the types and method to benchmark.
    /// </param>
    public void Run(Assembly assembly) {
      result_handler_.OnStartRun(options_);

      IEnumerable<Type> types =
        assembly
          .GetTypes()
          .OrderBy(type => type.FullName)
          .Where(type =>
            type
              .GetMethods(kAllInstance)
              .Any(IsBenchmark));

      foreach (Type type in types) {
        if (options_.TypeFilter != null && type.Name != options_.TypeFilter) {
          continue;
        }

        var ctor = type.GetConstructor(Type.EmptyTypes);
        if (ctor == null) {
          result_handler_.OnWarning(R.EmptyConstructorNotFound.Fmt(type.Name));
          continue;
        }

        result_handler_.OnStartType(type);
        object instance = ctor.Invoke(null);
        IEnumerable<MethodInfo> methods =
          type
            .GetMethods(kAllInstance)
            .Where(IsBenchmark)
            .Where(ShouldBenchmark);
        foreach (var method in methods) {
          BenchmarkResult result = RunBenchmark(method, instance, options_);
          if (result.Duration == TimeSpan.Zero) {
            result_handler_.OnWarning(R.ZeroDuration.Fmt(result.Method.Name));
          }
          result_handler_.OnResult(result);
        }
        result_handler_.OnEndType();
      }
      result_handler_.OnEndRun();
    }

    /// <summary>
    /// Verify if the give <paramref name="method"/> should be benchmarked.
    /// </summary>
    /// <param name="method">
    /// The method to verify.
    /// </param>
    /// <returns>
    /// <c>true</c> if the method should be benchmarked; otherwise, <c>false</c>.
    /// </returns>
    bool ShouldBenchmark(MethodInfo method) {
      HashSet<string> categories = GetCategories(method);
      List<string> include = options_.IncludedCategories;
      if (include != null && !categories.Overlaps(include)) {
        return false;
      }

      List<string> exclude = options_.ExcludedCategories;
      if (exclude != null && categories.Overlaps(exclude)) {
        return false;
      }

      string method_filter = options_.MethodFilter;
      if (method_filter != null) {
        return
          !method_filter.EndsWith("*")
            ? method.Name == method_filter
            : method
              .Name
              .StartsWith(method_filter.Substring(0, method_filter.Length - 1));
      }

      if (method.GetParameters().Length != 0) {
        result_handler_.OnWarning(R.MethodHasParameters.Fmt(method.Name));
        return false;
      }
      return true;
    }

    static BenchmarkResult RunBenchmark(MethodInfo method, object instance,
      BenchmarkOptions options) {
      var action =
        (Action) Delegate
          .CreateDelegate(typeof (Action), instance, method);

      if (options.DryRunOnly) {
        action();
        return new BenchmarkResult(method, 1, TimeSpan.FromTicks(1));
      }

      // Start small, double until we've hit our warm-up time
      int iterations = 100;
      while (true) {
        TimeSpan duration = RunTest(action, iterations);
        if (duration >= options.WarmUpTime) {
          // Scale up the iterations to work out the full test time
          double scale = ((double) options.TestTime.Ticks)/duration.Ticks;
          double scaled_iterations = scale*iterations;

          // Make sure we never end up overflowing...
          iterations = (int)Math.Min(scaled_iterations, int.MaxValue - 1);
          break;
        }
        // Make sure we don't end up overflowing due to doubling...
        if (iterations >= int.MaxValue/2) {
          break;
        }
        iterations *= 2;
      }
      TimeSpan test_duration = RunTest(action, iterations);
      return new BenchmarkResult(method, iterations, test_duration);
    }

    static TimeSpan RunTest(Action action, int iterations) {
      PrepareForTest();
      var timer = Stopwatch.StartNew();
      for (int i = 0; i < iterations; i++) {
        action();
      }
      return timer.Elapsed;
    }

    static void PrepareForTest() {
      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();
    }

    static HashSet<string> GetCategories(MethodInfo method) {
      Type attribute = typeof (CategoryAttribute);
      IEnumerable<string> categories =
        method
          .GetCustomAttributes(attribute, false)
          .Concat(method.DeclaringType.GetCustomAttributes(attribute, false))
          .Cast<CategoryAttribute>()
          .Select(c => c.Category);
      return new HashSet<string>(categories);
    }

    /// <summary>
    /// Checks if the given <paramref name="method"/> is annotated with the
    /// <see cref="BenchMarkAttribute"/> attribute.
    /// </summary>
    /// <returns>
    /// <c>true</c> if the given <paramref name="method"/> is annotated with
    /// the <see cref="BenchMarkAttribute"/> attribute; otherwise, <c>false</c>.
    /// </returns>
    static bool IsBenchmark(MethodInfo method) {
      return method.IsDefined(typeof (BenchMarkAttribute), false);
    }
  }
}
