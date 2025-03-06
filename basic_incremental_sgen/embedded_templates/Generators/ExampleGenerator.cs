using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CodeMechanic.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace embedded_templates;

[Generator]
public sealed class ExampleGenerator : IIncrementalGenerator
{
    private static Assembly my_ass = Assembly.GetExecutingAssembly();
    private static Assembly your_ass = Assembly.GetCallingAssembly();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(async ctx =>
        {
            string hell_code = await your_ass.ReadFile("HellWorld.template");

            var map = new Dictionary<string, string>()
            {
                [@"Hell"] = "Heck"
            };

            hell_code = hell_code
                .Split('\n')
                .ReplaceAll(map)
                .Rollup();

            var sourceText =
                !string.IsNullOrEmpty(hell_code)
                    ? hell_code
                    : $$"""
                        namespace SourceGeneratorInCSharp
                        {
                            public static class HelloWorld
                            {
                                public static void SayHello()
                                {
                                    Console.WriteLine("Hello");
                                }
                            }
                        }
                        """;
            ctx.AddSource("ExampleGenerator.g.cs",
                SourceText.From(sourceText, Encoding.UTF8));
        });
    }
}