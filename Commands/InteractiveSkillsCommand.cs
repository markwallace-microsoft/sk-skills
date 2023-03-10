namespace SemanticKernel.Prompts.Commands;

using Microsoft.SemanticKernel;
using System.CommandLine;

internal class InteractiveSkillsCommand : Command
{
    public InteractiveSkillsCommand(IKernel kernel) : base("interactive", "Run in interactive mode")
    {
        this.SetHandler(() => RunInteractiveSkills(kernel));
    }

    private void RunInteractiveSkills(IKernel kernel)
    {
    }
}
