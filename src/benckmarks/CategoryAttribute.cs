using System;

namespace Nohros.Metrics.Benchmarks
{
  /// <summary>
  /// Attibute applied to any benchmark method or class to specify that it
  /// belongs in a particular category.
  /// </summary>
  /// <remarks>
  /// Categories can be explicity included or excluded at execution time.
  /// When applied to a class, all benchmarks within that class are implicity
  /// in that category.
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
  public class CategoryAttribute : Attribute
  {
    public CategoryAttribute(string category) {
      Category = category;
    }

    public string Category { get; private set; }
  }
}
