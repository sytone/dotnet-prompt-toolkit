using DotnetPromptToolkit.Application;
using DotnetPromptToolkit.AutoSuggest;
using DotnetPromptToolkit.Completion;
using DotnetPromptToolkit.History;
using DotnetPromptToolkit.Validation;

namespace DotnetPromptToolkit.Shortcuts;

/// <summary>
/// Mirrors prompt_toolkit.shortcuts.prompt.prompt(). Convenience wrappers for
/// running a single PromptSession.
/// </summary>
public static class Prompt
{
    public static ValueTask<string> RunAsync(
        string message = "",
        IHistory? history = null,
        ICompleter? completer = null,
        IValidator? validator = null,
        IAutoSuggest? autoSuggest = null,
        CancellationToken cancellationToken = default)
    {
        var session = new PromptSession(history: history, completer: completer, validator: validator, autoSuggest: autoSuggest);
        return session.PromptAsync(message, cancellationToken);
    }

    /// <summary>
    /// Mirrors prompt_toolkit.shortcuts.prompt() with `is_password=True`. Returns the
    /// raw input but never echoes it. Echoing is the caller's responsibility.
    /// </summary>
    public static ValueTask<string> RunPasswordAsync(string message = "", CancellationToken cancellationToken = default)
    {
        Console.Write(message);
        return new ValueTask<string>(ReadPassword());

        static string ReadPassword()
        {
            var sb = new System.Text.StringBuilder();
            try
            {
                while (true)
                {
                    var info = Console.ReadKey(intercept: true);
                    if (info.Key == ConsoleKey.Enter) break;
                    if (info.Key == ConsoleKey.Backspace && sb.Length > 0) sb.Length--;
                    else if (!char.IsControl(info.KeyChar)) sb.Append(info.KeyChar);
                }
            }
            catch (InvalidOperationException)
            {
                // Console redirected; fall back to ReadLine.
                return Console.ReadLine() ?? string.Empty;
            }
            return sb.ToString();
        }
    }
}
