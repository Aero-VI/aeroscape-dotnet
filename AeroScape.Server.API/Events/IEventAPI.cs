namespace AeroScape.Server.API.Events;

/// <summary>
/// Event system interface for plugin event handling.
/// </summary>
public interface IEventAPI
{
    /// <summary>Register an event handler</summary>
    void RegisterHandler<TEvent>(EventHandler<TEvent> handler, EventPriority priority = EventPriority.Normal) 
        where TEvent : EventArgs;
    
    /// <summary>Unregister an event handler</summary>
    void UnregisterHandler<TEvent>(EventHandler<TEvent> handler) 
        where TEvent : EventArgs;
}

/// <summary>
/// Event priority determines the order in which handlers are called.
/// </summary>
public enum EventPriority
{
    /// <summary>Called first, can modify event data</summary>
    Lowest = 0,
    
    /// <summary>Called early, can modify event data</summary>
    Low = 1,
    
    /// <summary>Default priority, can modify event data</summary>
    Normal = 2,
    
    /// <summary>Called late, can modify event data</summary>
    High = 3,
    
    /// <summary>Called last, can modify event data</summary>
    Highest = 4,
    
    /// <summary>Called after all others, read-only (cannot cancel or modify)</summary>
    Monitor = 5
}

/// <summary>
/// Base class for cancellable events.
/// </summary>
public abstract class CancellableEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets whether this event is cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }
}