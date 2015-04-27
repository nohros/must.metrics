using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace Nohros.Metrics.Data
{
  /// <summary>
  /// Represents an item that can be searched for using a set of tags.
  /// </summary>
  public class TaggedItem
  {
    readonly List<Tag> tags_;
    readonly Lazy<BigInteger> id_;

    /// <summary>
    /// Initializes a new instance of the <see cref="TaggedItem"/> by
    /// using the given collection of <see cref="Tag"/>.
    /// </summary>
    /// <param name="tags">
    /// The tags to be associated with this item.
    /// </param>
    public TaggedItem(IEnumerable<Tag> tags) {
      if (tags == null) {
        throw new ArgumentNullException("tags");
      }

      tags_ = new List<Tag>(tags);
      id_ = new Lazy<BigInteger>(ComputeId);
    }

    /// <summary>
    /// Compute an identifier for a set of tags.
    /// </summary>
    /// <returns></returns>
    BigInteger ComputeId() {
      var sb = new StringBuilder();
      var tags = tags_.OrderBy(t => t.Name);
      foreach (Tag tag in tags) {
        sb.Append(tag.Name).Append("=").Append(tag.Value);
      }

      byte[] buffer = Encoding.UTF8.GetBytes(sb.ToString());
      HashAlgorithm sha1 = SHA1.Create();
      byte[] hash = sha1.ComputeHash(buffer);
      return new BigInteger(hash);
    }

    /// <summary>
    /// Gets a unique identifier based on the contained tags.
    /// </summary>
    /// <remarks>
    /// The <see cref="Id"/> is a SHA-1 hash of a normalized string
    /// representation and identical tags will always get the same id. Note
    /// that producing SHA-1 collision is not easy and no collision for SHA-1
    /// has been produced at yet(at the time of writting), it is possible
    /// for two distinct set of tags to produce the same hash. You should take
    /// that into account while using the <see cref="Id"/> to compare two
    /// <see cref="TaggedItem"/> for equality.
    /// </remarks>
    public BigInteger Id {
      get { return id_.Value; }
    }
  }
}
