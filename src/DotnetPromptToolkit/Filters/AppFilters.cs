namespace DotnetPromptToolkit.Filters;

/// <summary>
/// Application-aware filters mirroring prompt_toolkit.filters.app helpers
/// such as has_focus, is_done, has_completions, etc. They accept delegates
/// because the C# port doesn't yet have a global application context.
/// </summary>
public static class AppFilters
{
    public static Filter HasFocus(Func<bool> predicate) => new Condition(predicate);
    public static Filter IsDone(Func<bool> predicate) => new Condition(predicate);
    public static Filter HasCompletions(Func<bool> predicate) => new Condition(predicate);
    public static Filter HasSelection(Func<bool> predicate) => new Condition(predicate);
    public static Filter HasValidationError(Func<bool> predicate) => new Condition(predicate);
    public static Filter RendererHeightIsKnown(Func<bool> predicate) => new Condition(predicate);
    public static Filter InPasteMode(Func<bool> predicate) => new Condition(predicate);
    public static Filter ViMode(Func<bool> predicate) => new Condition(predicate);
    public static Filter EmacsMode(Func<bool> predicate) => new Condition(predicate);
}
