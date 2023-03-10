using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using SemanticKernel.Prompts.Utils;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace SemanticKernel.Prompts.Commands;

internal class RunSemanticFunctionCommand : Command
{
    private readonly Option<DirectoryInfo> location = new Option<DirectoryInfo>(new string[] { "--location", "-l" }, "Location of the skills folder");
    private readonly Option<string> skill = new Option<string>(new string[] { "--skill", "-s" }, "Name of the skill");
    private readonly Option<string> function = new Option<string>(new string[] { "--function", "-f" }, "Name of the function");

    public RunSemanticFunctionCommand(IKernel kernel) : base("runfunc", "Run semantic function")
    {
        location.IsRequired = true;

        this.AddOption(location);
        this.AddOption(skill);
        this.AddOption(function);

        this.SetHandler((InvocationContext context) => RunSkill(context, kernel));
    }

    private async Task RunSkill(InvocationContext context, IKernel kernel)
    {
        var parentDirectory = context.ParseResult.GetValueForOption(location);
        var skillName = context.ParseResult.GetValueForOption(skill);
        ArgumentNullException.ThrowIfNull(skillName);
        var functionName = context.ParseResult.GetValueForOption(function);
        ArgumentNullException.ThrowIfNull(functionName);

        kernel.ImportSemanticSkills(parentDirectory, skillName);
        var skfunction = (SKFunction)kernel.Func(skillName, functionName);

        await kernel.RunFunctionAsync(skfunction);
    }
}
