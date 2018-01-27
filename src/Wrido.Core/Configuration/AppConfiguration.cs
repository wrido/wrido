namespace Wrido.Configuration
{
  public interface IAppConfiguration
  {
    string HotKey { get; }
  }

  public class AppConfiguration : IAppConfiguration
  {
    public string HotKey { get; set; }
  }
}
