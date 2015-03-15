using System;
using Nohros.Configuration;

namespace Nohros.Metrics
{
  public partial class Settings : Configuration.Configuration
  {
    public Settings(Builder builder): base(builder) {
    }
  }
}