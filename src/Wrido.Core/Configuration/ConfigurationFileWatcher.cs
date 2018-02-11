using System;
using System.IO;
using System.Threading;
using Wrido.Logging;

namespace Wrido.Configuration
{
  public interface IConfigurationFileWatcher
  {
    event FileSystemEventHandler Updated;
  }

  internal class ConfigurationFileWatcher : IDisposable, IConfigurationFileWatcher
  {
    private readonly ILogger _logger;
    private readonly FileSystemWatcher _fileWatcher;
    public event FileSystemEventHandler Updated;
    private bool _eventDispatchQueued;
    private Timer _timer;
    private readonly object _dispatchLock = new object();

    public ConfigurationFileWatcher(ILogger logger)
    {
      _logger = logger;
      var cfgPath = Path.GetDirectoryName(ReadOnlyAppConfiguration.ConfigurationFilePath);
      var cfgFileName = Path.GetFileName(ReadOnlyAppConfiguration.ConfigurationFilePath);
      _logger.Information("Setting up file watcher on {configFileName} in {configDirectory}.", cfgFileName, cfgPath);
      _fileWatcher = new FileSystemWatcher
      {
        Path = cfgPath,
        Filter = cfgFileName,
        EnableRaisingEvents = true,
        IncludeSubdirectories = false,
        NotifyFilter = NotifyFilters.LastWrite
      };
      _fileWatcher.Changed += ThrottleAndDispatch;
      _fileWatcher.Created += ThrottleAndDispatch;
    }

    private void ThrottleAndDispatch(object sender, FileSystemEventArgs fileSystemEventArgs)
    {
      if (_eventDispatchQueued)
      {
        _logger.Verbose("Event dispatch is already queued.");
        return;
      }

      if (!Monitor.TryEnter(_dispatchLock))
      {
        _logger.Verbose("Another thread currently holds the lock to the event dispatcher");
        return;
      }
      
      _logger.Information("Configuration file updated. Preparing to raise event.");
      _eventDispatchQueued = true;

      _timer = new Timer(state =>
      {
        _logger.Information("Raising file updated event.");
        Updated?.Invoke(this, fileSystemEventArgs);
        _eventDispatchQueued = false;
      }, null, TimeSpan.FromMilliseconds(100), new TimeSpan(-1));
      Monitor.Exit(_dispatchLock);
    }

    public void Dispose()
    {
      _fileWatcher.Dispose();
      _timer?.Dispose();
    }
  }
}
