namespace DotnetPromptToolkit.PatchStdout;

/// <summary>
/// Mirrors prompt_toolkit.patch_stdout.patch_stdout. While active, lines
/// written to stdout are buffered so they don't break a rendered prompt and
/// are flushed when the patch is disposed.
/// </summary>
public sealed class PatchStdoutScope : IDisposable
{
    private readonly TextWriter _originalOut;
    private readonly StringWriter _buffer = new();

    public PatchStdoutScope()
    {
        _originalOut = Console.Out;
        Console.SetOut(_buffer);
    }

    public string GetBufferedOutput() => _buffer.ToString();

    public void Dispose()
    {
        Console.SetOut(_originalOut);
        _originalOut.Write(_buffer.ToString());
        _buffer.Dispose();
    }
}
