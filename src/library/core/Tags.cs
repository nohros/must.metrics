using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Nohros.Metrics
{
  public static class TagsExtensions
  {
    public static Tags ToTags(this IEnumerable<Tag> tags) {
      return new Tags(tags);
    }
  }

  /// <summary>
  /// Represents a set of <see cref="Tag"/>.
  /// </summary>
  /// <remarks>
  /// The <see cref="Tags"/> uses the <see cref=" Tag.Name"/> to compare for
  /// equality, which means that there is just one tag with a given name in
  /// the set.
  /// </remarks>
  /// <seealso cref="Tag"/>
  public class Tags : IEnumerable<Tag>
  {
    /// <summary>
    /// Provides a way to build a <see cref="Tags"/> object fluently.
    /// </summary>
    public class Builder
    {
      internal readonly List<Tag> tags_;

      /// <summary>
      /// Initializes a new instance of the <see cref="Builder"/> class.
      /// </summary>
      public Builder() : this(new Tag[0]) {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="Builder"/> class whose
      /// tags are copied from the given
      /// <paramref name="tags"/> object.
      /// </summary>
      /// <param name="tags">
      /// A <see cref="Tags"/> whose tags and values are copied to the
      /// <see cref="Builder"/>.
      /// </param>
      public Builder(Tags tags) : this((IEnumerable<Tag>) tags) {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="Builder"/> class whose
      /// tags are copied from the given
      /// <paramref name="tags"/> object.
      /// </summary>
      /// <param name="tags">
      /// A <see cref="Tags"/> whose tags and values are copied to the
      /// <see cref="Builder"/>.
      /// </param>
      public Builder(IEnumerable<Tag> tags) {
        tags_ = new List<Tag>(tags);
      }

      /// <summary>
      /// Adds a tag to the config.
      /// </summary>
      /// <param name="name">
      /// The tag's name
      /// </param>
      /// <param name="value">
      /// The tag's value.
      /// </param>
      public Builder WithTag(string name, string value) {
        if (name == null || value == null) {
          throw new ArgumentNullException(name == null ? "name" : "value");
        }
        tags_.Add(new Tag(name, value));
        return this;
      }

      /// <summary>
      /// Adds a tag to the config.
      /// </summary>
      /// <param name="tag">
      /// The tag to be added.
      /// </param>
      public Builder WithTag(Tag tag) {
        if (tag == null) {
          throw new ArgumentNullException("tag");
        }
        tags_.Add(tag);
        return this;
      }

      /// <summary>
      /// Adds a tag to the config.
      /// </summary>
      /// <param name="tags">
      /// The tag to be added.
      /// </param>
      public Builder WithTags(IEnumerable<Tag> tags) {
        if (tags == null) {
          throw new ArgumentNullException("tags");
        }

        // We  not using the tags.AddRange method to
        // ensure that none of the tags from the given
        // collection is null.
        foreach (var tag in tags) {
          WithTag(tag);
        }
        return this;
      }

      /// <summary>
      /// Creates a new <see cref="Tags"/> object containing the configured
      /// tags.
      /// </summary>
      /// <returns>
      /// The newly created <see cref="Tags"/> object.
      /// </returns>
      public Tags Build() {
        return new Tags(tags_);
      }
    }

    readonly Dictionary<string, Tag> tags_;

    /// <summary>
    /// Initializes a new instance of the <see cref="Tags"/> that contains no
    /// tags.
    /// </summary>
    public Tags() {
      tags_ = new Dictionary<string, Tag>();
      Id = Guid.NewGuid();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tags"/> class that
    /// contians elements copied from the specified collection of tags.
    /// </summary>
    /// <param name="tag">
    /// The single and unique tag of the collection.
    /// </param>
    public Tags(Tag tag) : this() {
      tags_[tag.Name] = tag;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Tags"/> class that
    /// contians elements copied from the specified collection of tags.
    /// </summary>
    /// <param name="tags">
    /// The collection whose elements are copied to the <see cref="Tags"/>
    /// collection.
    /// </param>
    public Tags(IEnumerable<Tag> tags) : this() {
      foreach (var tag in tags) {
        tags_[tag.Name] = tag;
      }
    }

    /// <summary>
    /// Determines whether a <see cref="Tags"/> object contains all the
    /// <see cref="Tag"/> of the specified tags collection.
    /// </summary>
    /// <param name="other">
    /// The collection to compare with the current <see cref="Tags"/> object.
    /// </param>
    /// <returns>
    /// <c>true</c> if the current <see cref="Tags"/> object contains all
    /// the <see cref="Tag"/> of the specified collection.
    /// </returns>
    public bool EqualsTo(IEnumerable<Tag> other) {
      return other.All(tag => tags_.ContainsKey(tag.Name));
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    /// <inheritdoc/>
    public IEnumerator<Tag> GetEnumerator() {
      return tags_.Values.GetEnumerator();
    }

    /// <summary>
    /// Gets a <see cref="Tags"/> object that contains no tags.
    /// </summary>
    public static Tags Empty {
      get { return new Tags(Enumerable.Empty<Tag>()); }
    }

    /// <summary>
    /// Gets a <see cref="Guid"/> that uniquely identifies the
    /// <see cref="Tags"/> object.
    /// </summary>
    /// <remarks>
    /// This <see cref="Id"/> should be used only as the <see cref="Tags"/>
    /// object id. This field should not be used to compare two tags
    /// for equality, because each object will have your own id.
    /// </remarks>
    public Guid Id { get; private set; }

    public int Count {
      get { return tags_.Count; }
    }
  }
}
