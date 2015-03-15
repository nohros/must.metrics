using System;
using Nohros.Configuration;

namespace Nohros.Metrics
{
  public partial class Settings
  {
    public class Loader : AbstractConfigurationLoader<Settings>
    {
      public Loader() :base(new Builder()) {
      }
    }
  }
}