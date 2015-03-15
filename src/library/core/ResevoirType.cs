using System;
using Nohros.Extensions;

namespace Nohros.Metrics.Annotations
{
  public static class ResevoirTypeExtensions
  {
    const string kDefaultName = "nhs.resevoir";

    /// <summary>
    /// Creates a <see cref="Tag"/> by using  the string "nohros.metrics.type"
    /// as the tag name and the given <paramref name="type"/> as the tag value.
    /// </summary>
    /// <param name="type">
    /// A <see cref="MetricType"/> that defines the value of the tag.
    /// </param>
    public static Tag AsTag(this ResevoirType type) {
      return AsTag(type, kDefaultName);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricTypeExtensions"/> by using 
    /// the given <paramref name="name"/> as the tag name and the
    /// <paramref name="type"/> as the tag value.
    /// </summary>
    /// <param name="type">
    /// A <see cref="MetricType"/> that defines the value of the tag.
    /// </param>
    /// <param name="name">
    /// The name of the tag.
    /// </param>
    public static Tag AsTag(this ResevoirType type, string name) {
      switch (type) {
        case ResevoirType.ExponentiallyDecaying:
          return new Tag(name, "exponentially-decaying");
        default:
          throw new ArgumentOutOfRangeException(
            Resources.ArgIsInvalid.Fmt((int) type, typeof (MetricType).Name));
      }
    }
  }

  public enum ResevoirType
  {
    ExponentiallyDecaying = 1
  }
}
