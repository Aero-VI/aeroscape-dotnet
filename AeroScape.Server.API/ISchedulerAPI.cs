namespace AeroScape.Server.API;

/// <summary>
/// Task scheduling API for plugins.
/// </summary>
public interface ISchedulerAPI
{
    /// <summary>
    /// Schedule a task to run after a delay.
    /// </summary>
    /// <param name="action">The action to run</param>
    /// <param name="delayTicks">Delay in server ticks (600ms each)</param>
    /// <returns>Task ID that can be used to cancel</returns>
    int RunTaskLater(Action action, int delayTicks);
    
    /// <summary>
    /// Schedule a task to run repeatedly.
    /// </summary>
    /// <param name="action">The action to run</param>
    /// <param name="delayTicks">Initial delay in server ticks</param>
    /// <param name="periodTicks">Period between executions in server ticks</param>
    /// <returns>Task ID that can be used to cancel</returns>
    int RunTaskTimer(Action action, int delayTicks, int periodTicks);
    
    /// <summary>
    /// Schedule an async task to run after a delay.
    /// </summary>
    /// <param name="func">The async function to run</param>
    /// <param name="delayTicks">Delay in server ticks</param>
    /// <returns>Task ID that can be used to cancel</returns>
    int RunTaskLaterAsync(Func<Task> func, int delayTicks);
    
    /// <summary>
    /// Schedule an async task to run repeatedly.
    /// </summary>
    /// <param name="func">The async function to run</param>
    /// <param name="delayTicks">Initial delay in server ticks</param>
    /// <param name="periodTicks">Period between executions in server ticks</param>
    /// <returns>Task ID that can be used to cancel</returns>
    int RunTaskTimerAsync(Func<Task> func, int delayTicks, int periodTicks);
    
    /// <summary>
    /// Cancel a scheduled task.
    /// </summary>
    /// <param name="taskId">The task ID returned when scheduling</param>
    /// <returns>True if task was cancelled, false if not found</returns>
    bool CancelTask(int taskId);
    
    /// <summary>
    /// Cancel all tasks for this plugin.
    /// </summary>
    void CancelAllTasks();
}