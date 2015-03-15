using System;
using System.Collections.Generic;

namespace Nohros.Metrics
{
  /// <summary>
  /// Defines the configuration settings that is associated with a metric.
  /// A <see cref="MetricConfig"/> consist of a required name and an optional
  /// set of tags.
  /// </summary>
  /// <seealso cref="Metrics.Tags"/>
  /// <seealso cref="Tag"/>
  public class MetricConfig
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="MetricConfig"/> class
    /// by using the given <paramref name="name"/> and a empty list of tags.
    /// </summary>
    /// <param name="name">
    /// The name of the metric.
    /// </param>
    public MetricConfig(string name) : this(name, Tags.Empty) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricConfig"/> class by
    /// using the given name and tags.
    /// </summary>
    /// <param name="name">
    /// A string that, in conjuction with the tags, uniquely identifies a
    /// metric.
    /// </param>
    /// <param name="tag">
    /// The single and unique tag of the config.
    /// </param>
    public MetricConfig(string name, Tag tag) : this(name, new Tags(tag)) {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MetricConfig"/> class by
    /// using the given name and tags.
    /// </summary>
    /// <param name="name">
    /// A string that, in conjuction with the tags, uniquely identifies a
    /// metric.
    /// </param>
    /// <param name="tags">
    /// A collection of key/value pairs that, in conjuction with the name
    /// uniquely identifies the metric.
    /// </param>
    public MetricConfig(string name, Tags tags) {
      Name = name;
      Tags = new Tags.Builder()
        .WithTags(tags)
        .Build();
    }

    /// <summary>
    /// Gets a copy of the current <see cref="MetricConfig"/> with an
    /// additional <see cref="Tag"/>
    /// </summary>
    /// <returns>
    /// A <see cref="MetricConfig"/> whose name is equals to
    /// <see cref="Name"/> and tags is a concatenation of the <see cref="Tags"/>
    /// and the given <paramref name="tag"/>
    /// </returns>
    public MetricConfig WithAdditionalTag(Tag tag) {
      return new MetricConfig(Name,
        new Tags.Builder(Tags)
          .WithTag(tag)
          .Build());
    }

    public MetricConfig WithAdditionalTag(string name, string value) {
      return new MetricConfig(Name,
        new Tags.Builder(Tags)
          .WithTag(new Tag(name, value))
          .Build());
    }

    /// <summary>
    /// Gets a copy of the current <see cref="MetricConfig"/> with an
    /// additional <see cref="Tags"/>
    /// </summary>
    /// <returns>
    /// A <see cref="MetricConfig"/> whose name is equals to
    /// <see cref="Name"/> and tags is a concatenation of the <see cref="Tags"/>
    /// and the given <paramref name="tags"/>
    /// </returns>
    public MetricConfig WithAdditionalTags(Tags tags) {
      return new MetricConfig(Name,
        new Tags.Builder(Tags)
          .WithTags(tags)
          .Build());
    }

    /// <summary>
    /// Gets a copy of the current <see cref="MetricConfig"/> with an
    /// additional tags.
    /// </summary>
    /// <returns>
    /// A <see cref="MetricConfig"/> whose name is equals to
    /// <see cref="Name"/> and tags is a concatenation of the <see cref="Tags"/>
    /// and the given <paramref name="tags"/>
    /// </returns>
    public MetricConfig WithAdditionalTags(IEnumerable<Tag> tags) {
      return new MetricConfig(Name,
        new Tags.Builder(Tags)
          .WithTags(tags)
          .Build());
    }

    /// <summary>
    /// A string that, associated with <see cref="Tags"/>, uniquely identifies
    /// a metric.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// A collection of key/value pairs that semantically identifies a
    /// metric.
    /// </summary>
    public Tags Tags { get; private set; }

    /// <inheritdoc/>
    public override bool Equals(object obj) {
      return Equals(obj as MetricConfig);
    }

    /// <summary>
    /// Determines whether the specified <paramref name="config"/> is equal
    /// to the current <see cref="MetricConfig"/> object.
    /// </summary>
    /// <param name="config">
    /// The <see cref="config"/> to compare with the current
    /// <see cref="MetricConfig"/> object.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="config"/> and the current
    /// <see cref="MetricConfig"/> object represents the same object;
    /// ohtherwise, <c>false</c>.
    /// </returns>
    public bool Equals(MetricConfig config) {
      if ((object) config == null) {
        return false;
      }

      return (config.Name == Name) && config.Tags.EqualsTo(Tags);
    }

    /// <inheritdoc/>
    public override int GetHashCode() {
      int hash = 17;
      hash = hash*23 + Name.GetHashCode();
      hash = hash*23 + Tags.GetHashCode();
      return hash;
    }
  }
}
