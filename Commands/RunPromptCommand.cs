using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using SemanticKernel.Prompts.Utils;
using System.CommandLine;
using System.CommandLine.Invocation;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SemanticKernelSample.Skills.Commands;

internal class RunPromptCommand : Command
{
    private readonly Option<DirectoryInfo> Location = CommandOptions.Location(true);
    private readonly Option<string> Prompt = CommandOptions.Prompt(true);
    private readonly Option<string> Function = CommandOptions.Function(true);

    public RunPromptCommand(IKernel kernel) : base("runprompt", "Run prompt")
    {
        this.AddOption(Location);
        this.AddOption(Prompt);
        this.AddOption(Function);

        this.SetHandler((InvocationContext context) => RunPromptAsync(context, kernel));
    }

    private async Task RunPromptAsync(InvocationContext context, IKernel kernel)
    {
        var parentDirectory = context.ParseResult.GetValueForOption(Location);
        var promptName = context.ParseResult.GetValueForOption(Prompt);
        ArgumentNullException.ThrowIfNull(promptName);
        var functionName = context.ParseResult.GetValueForOption(Function);
        ArgumentNullException.ThrowIfNull(functionName);

        kernel.ImportLangChainPrompts(parentDirectory, promptName);
        var skfunction = (SKFunction)kernel.Func(promptName, functionName);

        await kernel.RunFunctionAsync(skfunction);
    }
}
