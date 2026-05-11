namespace DotnetPromptToolkit;

public static class PromptEcho
{
    public static string RenderTranscript(string prompt, string input)
    {
        prompt ??= string.Empty;
        input ??= string.Empty;
        return $"{prompt}{input}{Environment.NewLine}";
    }
}
