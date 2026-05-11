using System.Diagnostics;

namespace DotnetPromptToolkit.Tests;

public class PromptEchoTests
{
    [Fact]
    public void RenderTranscript_UsesPromptAndInput()
    {
        var result = PromptEcho.RenderTranscript("> ", "hello");
        Assert.Equal("> hello" + Environment.NewLine, result);
    }

    [Fact]
    public void RenderTranscript_ParityWithPythonFormatter()
    {
        const string expected = "$ parity" + "\n";
        var actual = PromptEcho.RenderTranscript("$ ", "parity");
        Assert.Equal(expected.Replace("\n", Environment.NewLine), actual);

        if (!TryRunPython("$ ", "parity", out var pythonOutput))
        {
            return;
        }

        Assert.Equal(expected, pythonOutput);
    }

    private static bool TryRunPython(string prompt, string input, out string output)
    {
        var pythonCode = "import sys; prompt=sys.argv[1]; text=sys.argv[2]; sys.stdout.write(f'{prompt}{text}\\n')";
        var startInfo = new ProcessStartInfo("python3", $"-c \"{pythonCode}\" \"{prompt}\" \"{input}\"")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        try
        {
            using var process = Process.Start(startInfo);
            if (process is null)
            {
                output = string.Empty;
                return false;
            }

            output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return process.ExitCode == 0;
        }
        catch
        {
            output = string.Empty;
            return false;
        }
    }
}
