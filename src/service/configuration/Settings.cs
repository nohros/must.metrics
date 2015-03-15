using System;

namespace Nohros.Metrics
{
  public partial class Settings : Configuration.Configuration, ISettings
  {
    #region .ctor
    /// <summary>
    /// Initializes a new instance of the <see cref="Settings"/> class using
    /// the specified <see cref="Builder"/>.
    /// </summary>
    /// <param name="builder">
    /// A <see cref="Builder"/> containing the user configured settings.
    /// </param>
    public Settings(Builder builder) : base(builder) {
    }
    #endregion
  }
}
