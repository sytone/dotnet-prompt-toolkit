namespace DotnetPromptToolkit.EventLoop;

/// <summary>
/// Mirrors prompt_toolkit.eventloop.utils. Provides helpers to run code on the
/// event loop and to schedule background work.
/// </summary>
public static class EventLoopHelpers
{
    public static Task RunInBackgroundAsync(Func<Task> work, CancellationToken cancellationToken = default) =>
        Task.Run(work, cancellationToken);

    public static Task RunInExecutorAsync<T>(Func<T> work, CancellationToken cancellationToken = default) =>
        Task.Run(work, cancellationToken);

    public static async Task RunInTerminalAsync(Func<Task> action)
    {
        // python-prompt-toolkit suspends the renderer while running. We just await.
        await action().ConfigureAwait(false);
    }
}

/// <summary>
/// Mirrors prompt_toolkit.eventloop.inputhook.InputHookContext. The inputhook
/// is invoked while waiting for input so applications can pump foreign event
/// loops (e.g. Tk).
/// </summary>
public delegate void InputHook(InputHookContext context);

public sealed class InputHookContext
{
    public InputHookContext(CancellationToken inputAvailable)
    {
        InputAvailable = inputAvailable;
    }

    public CancellationToken InputAvailable { get; }
    public bool ShouldStop => InputAvailable.IsCancellationRequested;
}
