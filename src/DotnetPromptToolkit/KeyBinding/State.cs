namespace DotnetPromptToolkit.KeyBinding;

/// <summary>Mirrors prompt_toolkit.key_binding.vi_state.InputMode.</summary>
public enum ViInputMode
{
    Insert,
    InsertMultiple,
    Navigation,
    Replace,
    ReplaceSingle
}

/// <summary>Mirrors prompt_toolkit.key_binding.vi_state.CharacterFind.</summary>
public sealed record CharacterFind(char Character, bool Backwards = false);

/// <summary>Mirrors prompt_toolkit.key_binding.vi_state.ViState.</summary>
public sealed class ViState
{
    public ViInputMode InputMode { get; set; } = ViInputMode.Insert;
    public bool WaitingForDigraph { get; set; }
    public CharacterFind? LastCharacterFind { get; set; }
    public char? Operator { get; set; }
    public Dictionary<char, string> NamedRegisters { get; } = new();
    public bool TempNavigationMode { get; set; }
}

/// <summary>Mirrors prompt_toolkit.key_binding.emacs_state.EmacsState.</summary>
public sealed class EmacsState
{
    public bool IsRecordingMacro { get; set; }
    public List<KeyPress> CurrentRecording { get; } = new();
    public List<KeyPress>? Macro { get; set; }
    public string? CurrentPrefixArg { get; set; }
}
