using DotnetPromptToolkit.Application;
using DotnetPromptToolkit.Completion;
using DotnetPromptToolkit.Layout;

var example = args.FirstOrDefault() ?? "basic";

switch (example)
{
    case "completion":
        await CompletionExample();
        break;
    case "layout":
        LayoutExample();
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
