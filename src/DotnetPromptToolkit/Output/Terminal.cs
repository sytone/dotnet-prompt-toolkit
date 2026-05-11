namespace DotnetPromptToolkit.Output;

public interface ITerminalOutput
{
    void Write(string value);
    void WriteLine(string value);
}

public sealed class ConsoleTerminalOutput : ITerminalOutput
{
    public void Write(string value) => Console.Write(value);
    public void WriteLine(string value) => Console.WriteLine(value);
}

public static class Ansi
{
    public const string Reset = "\u001b[0m";
    public const string ClearScreen = "\u001b[2J";
    public static string MoveCursor(int row, int column) => $"\u001b[{row + 1};{column + 1}H";
}
