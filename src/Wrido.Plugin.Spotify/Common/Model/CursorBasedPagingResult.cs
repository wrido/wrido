using System;
using System.Collections.Generic;
using System.Text;

namespace Wrido.Plugin.Spotify.Common.Model
{
  public class CursorBasedPagingResult<TItem>
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

    public IDictionary<string, string> Cursors { get; set; }

    /// <summary>
    /// The total number of items available to return. 
    /// </summary>
    public int Total { get; set; }
  }
}
