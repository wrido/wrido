using System;
using System.Collections.Generic;

namespace Wrido.Plugin.Spotify.Common.Model.Full
{
  public class PublicUser
  {
    /// <summary>
    /// The name displayed on the user's profile. null if not available.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Known external URLs for this user.
    /// </summary>
    public IDictionary<string, Uri> ExternalUrls { get; set; }

    /// <summary>
    /// Information about the followers of this user. 
    /// </summary>
    public Followers Followers { get; set; }

    /// <summary>
    /// A link to the Web API endpoint for this user.
    /// </summary>
    public Uri Href { get; set; }

    /// <summary>
    /// The Spotify user ID for this user.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// The user's profile image.
    /// </summary>
    public IList<Image> Images { get; set; }

    /// <summary>
    /// The object type: "user" 
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// The Spotify URI for this user.
    /// </summary>
    public string Uri { get; set; }
  }
}