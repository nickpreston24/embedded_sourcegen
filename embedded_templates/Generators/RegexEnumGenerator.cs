using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CodeMechanic.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace embedded_templates;

[Generator]
public sealed class RegexEnumGenerator : IIncrementalGenerator
{
    private static Assembly my_ass = Assembly.GetExecutingAssembly();
    // private static Assembly your_ass = Assembly.GetCallingAssembly();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(async context =>
        {
            string template_code =
                await my_ass.ReadFile("RegexEnumBase.template");

            var patterns = new List<Pattern>()
            {
                new Pattern { name = "FoxRegexBase", pattern = @"\d+" },
                new Pattern { name = "HothEchoBase", pattern = @"\d{20,}+" }
            };

            foreach (var regex_pattern in patterns)
            {
                var replacements
                    = new Dictionary<string, string>()
                    {
                        [@"\$name\$"] = regex_pattern.name,
                        [@"\$pattern\$"] = regex_pattern.pattern,
                    };

                string code = template_code
                    .Split('\n')
                    .ReplaceAll(replacements)
                    .Rollup();

                context.AddSource($"{regex_pattern.name}.g.cs",
                    SourceText.From(code, Encoding.UTF8));
            }
        });
    }
}

internal class Pattern
{
    public string id { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string url { get; set; } = string.Empty;
    public string pattern { get; set; } = string.Empty;
}