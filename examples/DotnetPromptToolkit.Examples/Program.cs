using System.Text.Json;
using DotnetPromptToolkit.Application;
using DotnetPromptToolkit.Completion;
using DotnetPromptToolkit.FormattedText;
using DotnetPromptToolkit.History;
using DotnetPromptToolkit.Layout;
using PromptBuffer = DotnetPromptToolkit.Buffers.Buffer;
using PromptDocument = DotnetPromptToolkit.Buffers.Document;

var example = args.FirstOrDefault() ?? "basic";

switch (example)
{
    case "completion":
        await CompletionExample();
        break;
    case "layout":
        LayoutExample();
        break;
    case "parity":
        ParityScenario(args.Skip(1).FirstOrDefault() ?? "all");
        break;
    default:
        await BasicPromptExample();
        break;
}

static async Task BasicPromptExample()
{
    var session = new PromptSession();
    var name = await session.PromptAsync("Name: ");
    Console.WriteLine($"Hello {name}");
}

static async Task CompletionExample()
{
    var session = new PromptSession(completer: new WordCompleter(["red", "green", "blue"]));
    var value = await session.PromptAsync("Color: ");
    Console.WriteLine($"Selected {value}");
}

static void LayoutExample()
{
    var layout = new HSplit(new Label("dotnet-prompt-toolkit"), new TextArea(prompt: "> "));
    Console.WriteLine(layout.Render().ToPlainText());
}

static void ParityScenario(string scenario)
{
    var result = scenario switch
    {
        "document" => DocumentScenario(),
        "completion" => CompletionScenario(),
        "history" => HistoryScenario(),
        "formatted-text" => FormattedTextScenario(),
        "layout" => LayoutScenario(),
        "all" => new
        {
            scenario = "all",
            results = new object[]
            {
                DocumentScenario(),
                CompletionScenario(),
                HistoryScenario(),
                FormattedTextScenario(),
                LayoutScenario()
            }
        },
        _ => throw new ArgumentException($"Unknown parity scenario '{scenario}'.", nameof(scenario))
    };

    Console.WriteLine(JsonSerializer.Serialize(result, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
}

static object DocumentScenario()
{
    var document = new PromptDocument("alpha\nbeta gamma", 12);
    var position = document.TranslateIndexToPosition(document.CursorPosition);
    return new
    {
        scenario = "document",
        text = document.Text,
        cursorPosition = document.CursorPosition,
        row = position.Row,
        column = position.Column,
        currentLineBeforeCursor = document.CurrentLineBeforeCursor,
        wordBeforeCursor = document.GetWordBeforeCursor()
    };
}

static object CompletionScenario()
{
    var document = new PromptDocument("he", 2);
    var completer = new WordCompleter(["help", "hello", "world"]);
    var completions = completer.GetCompletionsAsync(document).AsTask().GetAwaiter().GetResult()
        .OrderBy(completion => completion.Text, StringComparer.Ordinal)
        .Select(completion => new
        {
            completion.Text,
            completion.StartPosition,
            Applied = completion.Apply(document)
        })
        .ToArray();

    return new { scenario = "completion", completions };
}

static object HistoryScenario()
{
    var history = new InMemoryHistory();
    var buffer = new PromptBuffer(history: history);
    buffer.InsertText("abc");
    buffer.Accept();
    return new { scenario = "history", entries = history.GetStrings().ToArray() };
}

static object FormattedTextScenario()
{
    var formatted = new FormattedText()
        .Append("hello", "class:greeting")
        .Append(" world");

    return new
    {
        scenario = "formatted-text",
        plainText = formatted.ToPlainText(),
        fragments = formatted.Fragments.Select(fragment => new { fragment.Style, fragment.Text }).ToArray()
    };
}

static object LayoutScenario()
{
    var layout = new HSplit(new Label("dotnet-prompt-toolkit"), new TextArea(prompt: "> "));
    return new { scenario = "layout", plainText = layout.Render().ToPlainText() };
}
