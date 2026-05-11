using DotnetPromptToolkit;

var prompt = args.Length > 0 ? args[0] : "> ";
var input = args.Length > 1 ? args[1] : "sample";
Console.Write(PromptEcho.RenderTranscript(prompt, input));
