using Microsoft.SemanticKernel;
using SemanticKernel.Prompts.Utils;
using System.CommandLine;
using System.CommandLine.Invocation;

namespace SemanticKernelSample.Skills.Commands;

internal class ImportSkillsCommand : Command
{
    private readonly Option<DirectoryInfo> Location = CommandOptions.Location(true);
    private readonly Option<string> Skill = CommandOptions.Skill();

    public ImportSkillsCommand(IKernel kernel) : base("import", "Import skills")
    {
        this.AddOption(Location);
        this.AddOption(Skill);

        this.SetHandler((InvocationContext context) => ImportSkills(context, kernel));
    }

    private void ImportSkills(InvocationContext context, IKernel kernel)
    {
        Console.WriteLine("Import skills");

        var parentDirectory = context.ParseResult.GetValueForOption(Location);
        var skillName = context.ParseResult.GetValueForOption(Skill);

        kernel.ImportSemanticSkills(parentDirectory, skillName);

        var functionsView = kernel.Skills.GetFunctionsView();
        functionsView.SemanticFunctions.ToList().ForEach(entry =>
        {
            Console.WriteLine(entry.Key);
            entry.Value.ForEach(functionView => Console.WriteLine($"  {functionView.Name}"));
        });
    }
}