using Funq;
using ServiceStack.WebHost.Endpoints;

namespace Nohros.Metrics.Api
{
  public class App : AppHostBase
  {
    public App() : base("Nohros Metrics", typeof(App).Assembly) {
    }

    public override void Configure(Container container) {
    }
  }
}