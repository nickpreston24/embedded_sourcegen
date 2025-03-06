using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using CodeMechanic.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace embedded_templates;

[Generator]
public sealed class RegexEnumGenerator : IIncrementalGenerator
{
    private static Assembly my_ass = Assembly.GetExecutingAssembly();
    private static Assembly your_ass = Assembly.GetCallingAssembly();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(async ctx =>
        {
            // string template_code =
            //     await your_ass.ReadFile("RegexEnumStruct.template");
            // string id = "sample";
            // string name = "FooRegexStruct";
            // string pattern = @"\d+";
            //
            // var replacements
            //     = new Dictionary<string, string>()
            //     {
            //         [@"$id$"] = id,
            //         [@"$name$"] = name,
            //         [@"$pattern$"] = pattern,
            //     };
            //
            // string code = template_code
            //     .Split('\n')
            //     .ReplaceAll(replacements)
            //     .Rollup();
            //
            // ctx.AddSource($"{name}.g.cs",
            //     SourceText.From(code, Encoding.UTF8));
        });
    }
}

public class RegexEnumParts
{
    public string id { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string url { get; set; } = string.Empty;
    public string pattern { get; set; } = string.Empty;
}