using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Nohros.Metrics.Annotations;

namespace Nohros.Metrics.Tests.Annotation
{
  public class AnnotatedMetricsTests
  {
    [Tag("global-tag-name", "global-tag-value")]
    class Annottated
    {
      [Tag("my-tag-name", "my-tag-value")]
      [Tag("my-second-tag-name", "my-second-tag-value")]
      readonly ITimer timer_;

      public Annottated(bool initialize = true) {
        if (initialize) {
          timer_ = StatsTimer.Create("latency");
        }
      }
    }

    [Test]
    public void should_register_metric_fields() {
      var annottated = new Annottated();
      ICompositeMetric metrics = AppMetrics.RegisterObject(annottated);

      Assert.That(metrics.Metrics.Count, Is.EqualTo(1));
    }

    [Test]
    public void should_add_class_tags_to_registered_metric() {
      var annottated = new Annottated();
      ICompositeMetric metrics = AppMetrics.RegisterObject(annottated);

      Assert.That(HasTag(metrics.Config.Tags, "global-tag-name"), Is.True);
    }

    [Test]
    public void should_add_field_tags_to_regitered_metrics() {
      var annottated = new Annottated();
      ICompositeMetric metrics = AppMetrics.RegisterObject(annottated);

      IMetric timer = metrics.Metrics.First();
      Assert.That(HasTag(timer.Config.Tags, "my-tag-name"), Is.True);
      Assert.That(HasTag(timer.Config.Tags, "my-second-tag-name"), Is.True);
    }

    [Test]
    public void should_not_allow_uninitialized_fields() {
      var annottated = new Annottated(false);
      Assert.Catch(() => AppMetrics.RegisterObject(annottated));
    }

    bool HasTag(IEnumerable<Tag> tags, string name) {
      return tags.Any(tag => tag.Name == name);
    }
  }
}
