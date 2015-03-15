using System;
using Nohros.Configuration;

namespace Nohros.Metrics
{
  public partial class Settings
  {
    public class Loader : AbstractConfigurationLoader<Settings>
    {
      #region .ctor
      /// <summary>
      /// Initializes a new instance of the <see cref="Loader"/> class.
      /// </summary>
      public Loader() : base(new Builder()) {
      }
      #endregion
    }
  }
}
