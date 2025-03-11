using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CodeMechanic.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace embedded_templates;

[Generator]
public sealed class DotEnvEnumGenerator : IIncrementalGenerator
{
    private static Assembly my_ass = Assembly.GetExecutingAssembly();
    private static Assembly your_ass = Assembly.GetCallingAssembly();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(async ctx =>
        {
            string template_code =
                await my_ass.ReadFile("dotenv.template");

            // string env_code = await your_ass.ReadFile(".env");

            string name = "DotEnvEnumTest";
            string pattern = @"\d+";
            string ns = "sharpify_web"; // your_ass.FullName;

            var replacements
                = new Dictionary<string, string>()
                {
                    [@"\$name\$"] = name,
                    [@"\$pattern\$"] = pattern,
                    [@"\$namespace\$"] = "fubar",
                    [@"\$comment\$"] = "dave isn't here, dude"
                };

            string code = template_code
                .Split('\n')
                .ReplaceAll(replacements)
                .Rollup();

            ctx.AddSource($"{name}.g.cs",
                SourceText.From(code, Encoding.UTF8));
        });
    }
}