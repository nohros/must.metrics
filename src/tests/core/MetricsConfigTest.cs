using System.Linq;
using NUnit.Framework;

namespace Nohros.Metrics.Tests
{
  public class MetricsConfigTest
  {
    [Test]
    public void should_copy_config_when_new_tag_is_added() {
      var config = new MetricConfig("test1");
      var config2 = config.WithAdditionalTag(new Tag("tag1", "tag1"));

      Assert.That(config, Is.Not.SameAs(config2));
    }

    [Test]
    public void should_copy_config_tags_when_new_is_added() {
      var config = new MetricConfig("test1", new Tag("tag1", "tag1"));
      var config2 = config.WithAdditionalTag(new Tag("tag2", "tag2"));

      Tag tag = config2.Tags.FirstOrDefault(x => x.Name == "tag1");
      Assert.That(tag, Is.Not.Null);
      Assert.That(tag.Name, Is.EqualTo("tag1"));

      tag = config2.Tags.FirstOrDefault(x => x.Name == "tag2");
      Assert.That(tag, Is.Not.Null);
      Assert.That(tag.Name, Is.EqualTo("tag2"));
    }
  }
}
