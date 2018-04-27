using Wrido.Queries.Events;

namespace Wrido.Configuration.Events
{
  public class ConfigurationAvailable : BackendEvent
  {
    public IAppConfiguration Configuration { get; set; }
  }
}
