namespace Wrido.Configuration
{
  public interface IAppConfiguration
  {
    string HotKey { get; }
  }

  internal class AppConfiguration : IAppConfiguration
  {
    public string HotKey { get; set; }
  }
}
