using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace Nohros.Metrics.Api
{
  [Route("/tags", "GET")]
  public class TagsRequest
  {
  }

  [Route("/tags/{TagName}", "GET")]
  public class TagRequest
  {
    public string TagName { get; set; }
  }

  public class TagsResponse
  {
  }

  public class TagsApi : Service
  {
    public string Get(TagsRequest request) {
      return "tags";
    }

    public string Get(TagRequest request) {
      return Request.QueryString.Count.ToString();
    }
  }
}
