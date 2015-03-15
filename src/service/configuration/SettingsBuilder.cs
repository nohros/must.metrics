using System;
using System.Collections.Generic;
using Nohros.Configuration.Builders;

namespace Nohros.Metrics
{
  public partial class Settings
  {
    public class Builder : AbstractConfigurationBuilder<Settings>
    {
      public override Settings Build() {
        return new Settings(this);
      }
    }
  }
}
