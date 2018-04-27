using Wrido.Queries.Events;

namespace Wrido.Configuration.Events
{
  public class ConfigurationUpdated : BackendEvent
  {
    public IAppConfiguration Configuration { get; set; }
  }
}
