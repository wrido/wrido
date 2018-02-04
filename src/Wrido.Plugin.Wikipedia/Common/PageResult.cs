using System;
using System.Collections.Generic;

namespace Wrido.Plugin.Wikipedia.Common
{
  public class PageResult
  {
    public long PageId { get; set; }
    public string Title { get; set; }
    public Image Original { get; set; }
    public string Extract { get; set; }
    public Dictionary<DateTime, long?> PageViews { get; set; }
    public PageTerms Terms { get; set; }

    public class PageTerms
    {
      public List<string> Description { get; set; }
      public List<string> Label { get; set; }
      public List<string> Alias { get; set; }
    }

    public class Image
    {
      public Uri Source { get; set; }
      public int Width { get; set; }
      public int Height { get; set; }
    }
  }
}
