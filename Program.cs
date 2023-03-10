using SemanticKernel.Prompts.Commands;
using SemanticKernel.Prompts.Utils;
using System.CommandLine;

var kernel = KernelUtils.CreateKernel();

var rootCommand = new RootCommand();
rootCommand.AddImportSkillsCommand(kernel);
rootCommand.AddRunSemanticFunctionCommand(kernel);
rootCommand.AddImportPromptsCommand(kernel);
rootCommand.AddRunPromptCommand(kernel);
rootCommand.AddInteractiveSkillsCommand(kernel);

return await rootCommand.InvokeAsync(args);