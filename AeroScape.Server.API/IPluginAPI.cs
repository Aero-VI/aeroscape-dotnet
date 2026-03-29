using Microsoft.Extensions.Logging;
using AeroScape.Server.API.Events;

namespace AeroScape.Server.API;

/// <summary>
/// Main API surface exposed to plugins.
/// </summary>
public interface IPluginAPI
{
    /// <summary>Event system for hooking game events</summary>
    IEventAPI Events { get; }
    
    /// <summary>Player management and queries</summary>
    IPlayerAPI Players { get; }
    
    /// <summary>World and object interactions</summary>
    IWorldAPI World { get; }
    
    /// <summary>Item definitions and management</summary>
    IItemAPI Items { get; }
    
    /// <summary>Skill system access</summary>
    ISkillAPI Skills { get; }
    
    /// <summary>Inventory management</summary>
    IInventoryAPI Inventory { get; }
    
    /// <summary>Packet sending and interception</summary>
    IPacketAPI Packets { get; }
    
    /// <summary>Plugin configuration management</summary>
    IConfigAPI Config { get; }
    
    /// <summary>Scheduled task execution</summary>
    ISchedulerAPI Scheduler { get; }
    
    /// <summary>Plugin-specific storage</summary>
    IStorageAPI Storage { get; }
    
    /// <summary>Logging for this plugin</summary>
    ILogger Logger { get; }
}