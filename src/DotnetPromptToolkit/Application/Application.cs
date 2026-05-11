using PromptBuffer = DotnetPromptToolkit.Buffers.Buffer;
using DotnetPromptToolkit.Completion;
using DotnetPromptToolkit.History;
using DotnetPromptToolkit.Validation;

namespace DotnetPromptToolkit.Application;

public sealed class PromptSession
{
    public PromptSession(IHistory? history = null, ICompleter? completer = null, IValidator? validator = null)
    {
        Buffer = new PromptBuffer(history: history, completer: completer, validator: validator);
    }

    public PromptBuffer Buffer { get; }

    public async ValueTask<string> PromptAsync(string message = "", CancellationToken cancellationToken = default)
    {
        Console.Write(message);
        var line = await Console.In.ReadLineAsync(cancellationToken).ConfigureAwait(false) ?? string.Empty;
        Buffer.Reset(line);
        Buffer.Accept();
        return line;
    }
}
