using System;
using System.Collections.Generic;

namespace Wrido.Plugin.Spotify.Common.Model
{
  public class PaginationResult<TItem>
  {
    /// <summary>
    /// A link to the Web API endpoint returning the full result of the request.
    /// </summary>
    public string Href { get; set; }

    /// <summary>
    /// 	The requested data.
    /// </summary>
    public IList<TItem> Items { get; set; }

    /// <summary>
    /// The maximum number of items in the response (as set in the query or by default).
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// URL to the next page of items. (null if none) 
    /// </summary>
    public Uri Next { get; set; }

    /// <summary>
    /// The offset of the items returned (as set in the query or by default).
    /// </summary>
    public int Offset { get; set; }

    /// <summary>
    /// URL to the previous page of items. (null if none) 
    /// </summary>
    public Uri Previous { get; set; }

    /// <summary>
    /// The total number of items available to return. 
    /// </summary>
    public int Total { get; set; }
  }
}
