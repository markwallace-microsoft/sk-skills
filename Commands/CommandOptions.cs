namespace SemanticKernelSample.Skills.Commands;

using System.CommandLine;

static internal class CommandOptions
{
    internal static Option<DirectoryInfo> Location(bool IsRequired = false) => CreateOption<DirectoryInfo>(new string[] { "--location", "-l" }, "Location of the prompts folder", IsRequired);
    internal static Option<string> Prompt(bool IsRequired = false) => CreateOption<string>(new string[] { "--prompt", "-p" }, "Name of the prompt", IsRequired);
    internal static Option<string> Skill(bool IsRequired = false) => CreateOption<string>(new string[] { "--skill", "-s" }, "Name of the skill", IsRequired);
    internal static Option<string> Function(bool IsRequired = false) => CreateOption<string>(new string[] { "--function", "-f" }, "Name of the function", IsRequired);

    internal static Option<T> CreateOption<T>(string[] aliases, string description, bool IsRequired)
    {
        var option = new Option<T>(aliases, description);
        option.IsRequired = IsRequired;
        return option;
    }
}
