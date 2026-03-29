using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using AeroScape.Server.API.Events;

namespace AeroScape.Server.PluginHost;

/// <summary>
/// Manages event registration and dispatching for plugins.
/// </summary>
public class EventManager : IEventAPI
{
    private readonly ILogger<EventManager> _logger;
    private readonly ConcurrentDictionary<Type, List<RegisteredHandler>> _handlers = new();
    private readonly string _pluginName;

    private record RegisteredHandler(
        Delegate Handler,
        EventPriority Priority,
        string PluginName,
        bool IsMonitor);

    public EventManager(ILogger<EventManager> logger, string pluginName)
    {
        _logger = logger;
        _pluginName = pluginName;
    }

    public void RegisterHandler<TEvent>(EventHandler<TEvent> handler, EventPriority priority = EventPriority.Normal) 
        where TEvent : EventArgs
    {
        var eventType = typeof(TEvent);
        var handlers = _handlers.GetOrAdd(eventType, _ => new List<RegisteredHandler>());
        
        lock (handlers)
        {
            handlers.Add(new RegisteredHandler(handler, priority, _pluginName, priority == EventPriority.Monitor));
            
            // Re-sort by priority
            handlers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }
        
        _logger.LogDebug("Plugin {Plugin} registered handler for {Event} at priority {Priority}", 
            _pluginName, eventType.Name, priority);
    }

    public void UnregisterHandler<TEvent>(EventHandler<TEvent> handler) 
        where TEvent : EventArgs
    {
        var eventType = typeof(TEvent);
        if (!_handlers.TryGetValue(eventType, out var handlers))
            return;
            
        lock (handlers)
        {
            handlers.RemoveAll(h => h.Handler.Equals(handler));
        }
    }

    /// <summary>
    /// Fire an event to all registered handlers.
    /// </summary>
    public void FireEvent<TEvent>(object? sender, TEvent args) where TEvent : EventArgs
    {
        var eventType = typeof(TEvent);
        if (!_handlers.TryGetValue(eventType, out var handlers))
            return;

        List<RegisteredHandler> handlersSnapshot;
        lock (handlers)
        {
            handlersSnapshot = handlers.ToList();
        }

        foreach (var registeredHandler in handlersSnapshot)
        {
            try
            {
                // Check if event is already cancelled and this isn't a monitor
                if (args is CancellableEventArgs cancellable && 
                    cancellable.IsCancelled && 
                    !registeredHandler.IsMonitor)
                {
                    continue;
                }

                // Monitors cannot modify the event
                if (registeredHandler.IsMonitor && args is CancellableEventArgs)
                {
                    // Could create a read-only wrapper, but for now just trust plugins
                }

                var handler = (EventHandler<TEvent>)registeredHandler.Handler;
                handler(sender, args);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Plugin {Plugin} threw exception in event handler for {Event}", 
                    registeredHandler.PluginName, eventType.Name);
            }
        }
    }

    /// <summary>
    /// Unregister all handlers for a specific plugin.
    /// </summary>
    public void UnregisterAllForPlugin(string pluginName)
    {
        foreach (var kvp in _handlers)
        {
            lock (kvp.Value)
            {
                kvp.Value.RemoveAll(h => h.PluginName == pluginName);
            }
        }
        
        _logger.LogDebug("Unregistered all event handlers for plugin {Plugin}", pluginName);
    }
}