using System;
using System.Web;

namespace Nohros.Metrics
{
  public class MetricsHttpHandler : IHttpHandler
  {
    public void ProcessRequest(HttpContext context) {
    }

    public bool IsReusable {
      get { return false; }
    }
  }
}
