
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.KernelExtensions;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.SemanticFunctions;
using Microsoft.SemanticKernel.SkillDefinition;
using SemanticKernelSample.Skills.LangChain;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SemanticKernel.Prompts.Utils;

internal static class KernelUtils
{
    static readonly ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
    {
        builder
            .AddFilter("Microsoft", LogLevel.Warning)
            .AddFilter("System", LogLevel.Warning)
            .AddConsole();
    });

    internal static IKernel CreateKernel()
    {
        var logger = loggerFactory.CreateLogger<Program>();
        var kernel = Kernel.Builder.WithLogger(logger).Build();

        if (!string.IsNullOrEmpty(EnvVar("OPENAI_LABEL", false)))
        {
            kernel.Log.LogTrace("Adding OpenAI backend");
            kernel.Config.AddOpenAICompletionBackend(EnvVar("OPENAI_LABEL"), EnvVar("OPENAI_MODEL_ID"), EnvVar("OPENAI_API_KEY"));
        }

        if (!string.IsNullOrEmpty(EnvVar("AZURE_OPENAI_DEPLOYMENT_LABEL", false)))
        {
            kernel.Log.LogTrace("Adding Azure OpenAI backend");
            kernel.Config.AddAzureOpenAICompletionBackend(EnvVar("AZURE_OPENAI_DEPLOYMENT_LABEL"), EnvVar("AZURE_OPENAI_DEPLOYMENT_NAME"), EnvVar("AZURE_OPENAI_API_ENDPOINT"), EnvVar("AZURE_OPENAI_API_KEY"));
        }

        return kernel;
    }

    internal static void ImportSemanticSkills(this IKernel kernel, DirectoryInfo? parentDirectory, string? skillName = null)
    {
        if (parentDirectory == null)
        {
            Console.Error.WriteLine("Import location must be provided.");
            return;
        }

        if (!parentDirectory.Exists)
        {
            Console.Error.WriteLine($"Import location {parentDirectory.FullName} does not exist.");
            return;
        }

        if (skillName == null)
        {
            string[] directories = Directory.GetDirectories(parentDirectory.FullName);
            foreach (string directory in directories)
            {
                kernel.ImportSemanticSkillFromDirectory(parentDirectory.FullName, directory.Split('\\').Last());
            }
        }
        else
        {
            kernel.ImportSemanticSkillFromDirectory(parentDirectory.FullName, skillName);
        }
    }

    internal static void ImportLangChainPrompts(this IKernel kernel, DirectoryInfo? parentDirectory, string? promptName = null)
    {
        if (parentDirectory == null)
        {
            Console.Error.WriteLine("Import location must be provided.");
            return;
        }

        if (!parentDirectory.Exists)
        {
            Console.Error.WriteLine($"Import location {parentDirectory.FullName} does not exist.");
            return;
        }

        if (promptName == null)
        {
            string[] directories = Directory.GetDirectories(parentDirectory.FullName);
            foreach (string directory in directories)
            {
                kernel.ImportPromptsFromDirectory(parentDirectory.FullName, directory.Split('\\').Last());
            }
        }
        else
        {
            kernel.ImportPromptsFromDirectory(parentDirectory.FullName, promptName);
        }
    }

    public static IDictionary<string, ISKFunction> ImportPromptsFromDirectory(this IKernel kernel, string parentDirectory, string promptDirectoryName)
    {
        var skill = new Dictionary<string, ISKFunction>();
        var promptDir = Path.Join(parentDirectory, promptDirectoryName);
        if (!Directory.Exists(promptDir))
        {
            Console.Error.WriteLine($"Import location {promptDir} does not exist.");
            return skill;
        }

        string[] files = Directory.GetFiles(promptDir, "*.json", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            // Load prompt template
            var prompt = LangChainPrompt.FromJson(File.ReadAllText(file));
            if (prompt == null) continue;

            var config = new PromptTemplateConfig();
            var template = new PromptTemplate(prompt.ConvertPrompt(), config, kernel.PromptTemplateEngine);

            // Prepare lambda wrapping AI logic
            var functionConfig = new SemanticFunctionConfig(config, template);

            var functionName = PromptFileToFunctionName(promptDir, file);
            kernel.Log.LogTrace("Registering function {0}.{1} loaded from {2}", promptDirectoryName, functionName, file);
            skill[functionName] = kernel.RegisterSemanticFunction(promptDirectoryName, functionName, functionConfig);
        }

        files = Directory.GetFiles(promptDir, "*.yaml", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            // Load prompt template
            var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
            var prompt = deserializer.Deserialize<LangChainPrompt>(File.ReadAllText(file));
            var config = new PromptTemplateConfig();
            var template = new PromptTemplate(prompt.ConvertPrompt(), config, kernel.PromptTemplateEngine);

            // Prepare lambda wrapping AI logic
            var functionConfig = new SemanticFunctionConfig(config, template);

            var functionName = PromptFileToFunctionName(promptDir, file);
            kernel.Log.LogTrace("Registering function {0}.{1} loaded from {2}", promptDirectoryName, functionName, file);
            var skillName = PromptDirectoryToSkillName(promptDirectoryName);
            skill[functionName] = kernel.RegisterSemanticFunction(skillName, functionName, functionConfig);
        }

        return skill;
    }

    public static async Task RunFunctionAsync(this IKernel kernel, SKFunction skfunction)
    {
        Console.WriteLine(skfunction.Description);

        var input = string.Empty;
        if (skfunction.Parameters.HasInputParameter())
        {
            Console.Write("Enter input > ");
            input = Console.ReadLine() ?? string.Empty;
        }

        var variables = new ContextVariables(input);
        if (skfunction.Parameters.Count > 0)
        {
            // prompt for the parameters
            foreach (var parameter in skfunction.Parameters)
            {
                if ("input".Equals(parameter.Name))
                    continue;

                Console.Write($"Enter {parameter.Name} > ");
                var value = Console.ReadLine();
                variables.Set(parameter.Name, value);
            }
        }

        var response = await kernel.RunAsync(variables, skfunction);

        Console.WriteLine(response.Result);
    }

    internal static string PromptFileToFunctionName(string promptDir, string file)
    {
        var parts = file.Split(Path.DirectorySeparatorChar);
        return PromptDirectoryToSkillName(parts[parts.Length - 2]) + "_" + Path.GetFileNameWithoutExtension(file);
    }

    internal static string PromptDirectoryToSkillName(string directory)
    {
        if (IsValidSkillName(directory))
        {
            return directory;
        }

        // TODO handle all invalid characters
        return directory.Replace("-", "_");
    }

    internal static bool IsValidSkillName(string? skillName)
    {
        if (string.IsNullOrWhiteSpace(skillName))
        {
            return false;
        }

        Regex pattern = new("^[0-9A-Za-z_]*$");
        return pattern.IsMatch(skillName);
    }


    internal static bool HasInputParameter(this IList<ParameterView> parameters)
    {
        return parameters.Where(parameter => parameter.Name == "input").Count() > 0;
    }

    internal static string EnvVar(string name, bool throwExceptionIfNullOrEmpty = true)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (throwExceptionIfNullOrEmpty)
        {
            if (string.IsNullOrEmpty(value)) throw new Exception($"Env var not set: {name}");
        }
        return value ?? string.Empty;
    }

}
