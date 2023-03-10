namespace SemanticKernel.Prompts.Commands;

using Microsoft.SemanticKernel;
using SemanticKernelSample.Skills.Commands;
using System.CommandLine;

public static class RootCommandExtensions
{
    public static void AddInteractiveSkillsCommand(this RootCommand root, IKernel kernel) => root.AddCommand(new InteractiveSkillsCommand(kernel));

    public static void AddRunSemanticFunctionCommand(this RootCommand root, IKernel kernel) => root.AddCommand(new RunSemanticFunctionCommand(kernel));

    public static void AddImportSkillsCommand(this RootCommand root, IKernel kernel) => root.AddCommand(new ImportSkillsCommand(kernel));

    public static void AddImportPromptsCommand(this RootCommand root, IKernel kernel) => root.AddCommand(new ImportPromptsCommand(kernel));

    public static void AddRunPromptCommand(this RootCommand root, IKernel kernel) => root.AddCommand(new RunPromptCommand(kernel));
}
