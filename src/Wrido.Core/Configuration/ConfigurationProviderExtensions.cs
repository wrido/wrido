namespace Wrido.Configuration
{
  public static class ConfigurationProviderExtensions
  {
    /// <summary>
    /// Loads plugin configuration by assuming the plugin name is the same as the namespace of the configuration object
    /// </summary>
    /// <typeparam name="TPlugin">The plugin configuration object.</typeparam>
    /// <param name="provider">The provider.</param>
    /// <returns>The plugin configuration object, or default.</returns>
    public static TPlugin GetConfiguration<TPlugin>(this IConfigurationProvider provider) where TPlugin : IPluginConfiguration
    {
      return provider.GetConfiguration<TPlugin>(typeof(TPlugin).Namespace);
    }

    public static TPlugin GetConfiguration<TPlugin>(this IConfigurationProvider provider, string pluginName) where TPlugin : IPluginConfiguration
    {
      return provider.TryGetConfiguration(pluginName, out TPlugin plugin) ? plugin : default;
    }
  }
}
