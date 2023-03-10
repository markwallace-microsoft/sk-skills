namespace SemanticKernelSample.Skills.LangChain;

using System.Text.Json;
using System.Text.Json.Serialization;

internal class LangChainPrompt
{
    [JsonPropertyName("input_variables")]
    public string[] InputVariables { get; set; } = new string[0];

    [JsonPropertyName("output_parser")]
    public string? OutputParser { get; set; }

    [JsonPropertyName("template")]
    public string? Template { get; set; }

    [JsonPropertyName("template_format")]
    public string? TemplateFormat { get; set; }

    public string ConvertPrompt()
    {
        if (string.IsNullOrEmpty(Template)) return string.Empty;

        // TODO check the template format, options are: 'f-string', 'jinja2'
        var converted = Template.Replace("{{", "{").Replace("}}", "}");
        foreach (var item in InputVariables)
        {
            converted = converted.Replace($"{{{item}}}", $"{{{{${item}}}}}");
        }
        return converted;
    }

    public static LangChainPrompt? FromJson(string json)
    {
        return JsonSerializer.Deserialize<LangChainPrompt>(json);
    }
}
