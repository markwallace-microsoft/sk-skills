using Microsoft.SemanticKernel;
using SemanticKernel.Prompts.Utils;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace SemanticKernelSample.Skills.Commands;

internal class ImportPromptsCommand : Command
{
    private readonly Option<DirectoryInfo> Location = CommandOptions.Location(true);
    private readonly Option<string> Prompt = CommandOptions.Prompt();

    public ImportPromptsCommand(IKernel kernel) : base("importprompts", "Import prompts")
    {
        this.AddOption(Location);
        this.AddOption(Prompt);

        this.SetHandler((InvocationContext context) => ImportPrompts(context, kernel));
    }

    private void ImportPrompts(InvocationContext context, IKernel kernel)
    {
        Console.WriteLine("Import prompts");

        var parentDirectory = context.ParseResult.GetValueForOption(Location);
        var promptName = context.ParseResult.GetValueForOption(Prompt);

        kernel.ImportLangChainPrompts(parentDirectory, promptName);

        var functionsView = kernel.Skills.GetFunctionsView();
        functionsView.SemanticFunctions.ToList().ForEach(entry =>
        {
            Console.WriteLine(entry.Key);
            entry.Value.ForEach(functionView => Console.WriteLine($"  {functionView.Name}"));
        });
    }
}