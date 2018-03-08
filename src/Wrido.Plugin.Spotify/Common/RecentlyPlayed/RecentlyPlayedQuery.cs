using System;

namespace Wrido.Plugin.Spotify.Common.RecentlyPlayed
{
  public class RecentlyPlayedQuery
  {
    /// <summary>
    /// Optional. The maximum number of items to return. Default: 20. Minimum: 1. Maximum: 50. 
    /// </summary>
    public ushort Limit { get; set; }

    public DateFilter DateFilter { get; set; }

    public static RecentlyPlayedQuery Default => new RecentlyPlayedQuery
    {
      Limit = 10
    };
  }

  public abstract class DateFilter
  {
    public DateTime Date { get; set; }
  }

  public class After : DateFilter
  {
    public After(DateTime date) => Date = date;
  }

  public class Before : DateFilter
  {
    public Before(DateTime date) => Date = date;
  }
}
