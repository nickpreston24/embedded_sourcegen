using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace DemoSourceGenerator;

[Generator]
public class DemoSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        string code = @"""
                        namespace DemoSourceGenerator;
                        public class fubar {}
                      """;
        string name = "fubar";

        var additional_texts = context.AdditionalTextsProvider.Collect();

        var compilationAndAdditionaltexts =
            context.CompilationProvider.Combine(additional_texts);

        context.RegisterImplementationSourceOutput(
            additional_texts,
            async (source_context, source) =>
            {
                source_context.AddSource($"{name}.g.cs",
                    SourceText.From(code, Encoding.UTF8));
            });
    }
}