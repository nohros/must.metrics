using System;

namespace Nohros.Metrics
{
  /// <summary>
  /// Represents a semantic tag.
  /// </summary>
  /// <remarks>
  /// Tags allow the natural grouping of similar metrics, wich give users the
  /// ability to ealisy correlate freely between the metrics that incorporate
  /// them.
  /// 
  /// For example, take the following two metrics
  /// 
  ///  * nohros.com.interface.traffic.eth0.in
  ///  * nohros.com.interface.traffic.eth0.out
  /// 
  /// Tags simplifies that representation, these would be represented as:
  /// 
  ///  * interface.traffic {host=nohros.com, interface=eth0, direction=in}
  ///  * interface.traffic {host=nohros.com, interface=eth0, direction=out}
  /// </remarks>
  public class Tag
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="Tag"/> class by using the
    /// given tag name and value.
    /// </summary>
    /// <param name="name">
    /// The tag's name.
    /// </param>
    /// <param name="value">
    /// The tag's value.
    /// </param>
    public Tag(string name, string value) {
      if (name == null || value == null) {
        throw new ArgumentNullException(name == null ? "name" : "value");
      }
      Name = name;
      Value = value;
    }

    /// <inheritdoc/>
    public override string ToString() {
      return Name + ":" + Value;
    }

    /// <inheritdoc/>
    public override bool Equals(object obj) {
      return Equals(obj as Tag);
    }

    /// <summary>
    /// Determines whether the specified <paramref name="tag"/> is equal to the
    /// current <see cref="Tag"/> object.
    /// </summary>
    /// <param name="tag">
    /// The <see cref="Tag"/> to compare with the current <see cref="Tag"/>
    /// object.
    /// </param>
    /// <returns>
    /// <c>true</c> if <paramref name="tag"/> and the current <see cref="Tag"/>
    /// object represents the same object; ohtherwise, <c>false</c>.
    /// </returns>
    public bool Equals(Tag tag) {
      if ((object) tag == null) {
        return false;
      }
      return Name == tag.Name && Value == tag.Value;
    }

    /// <inheritdoc/>
    public override int GetHashCode() {
      int hash = 17;
      hash = hash*23 + Name.GetHashCode();
      hash = hash*23 + Value.GetHashCode();
      return hash;
    }

    /// <summary>
    /// Gets or sets the name of the tag.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Gets or sets the value of the tag.
    /// </summary>
    public string Value { get; protected set; }
  }
}
