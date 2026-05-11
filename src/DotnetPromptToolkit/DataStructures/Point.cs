namespace DotnetPromptToolkit.DataStructures;

/// <summary>Mirrors prompt_toolkit.data_structures.Point.</summary>
public readonly record struct Point(int X, int Y);

/// <summary>Mirrors prompt_toolkit.data_structures.Size.</summary>
public readonly record struct Size(int Rows, int Columns);
