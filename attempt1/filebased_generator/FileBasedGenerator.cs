using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

[Generator]
public class FileBasedGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // Read the content of MyClass.cs
        var sourceFile = context.AdditionalFiles.FirstOrDefault(file => file.Path.EndsWith("MyClass.cs"));

        if (sourceFile != null)
        {
            var fileContent = sourceFile.GetText(context.CancellationToken)?.ToString();

            // Generate additional code based on the content of MyClass.cs
            if (fileContent != null)
            {
                var generatedCode =
                    $@" namespace MyNamespace.Generated {{ public static class MyClassExtensions {{ public static void PrintInfo(this MyClass instance) {{ Console.WriteLine(""MyMethod was called in MyClass""); }} }} }}";

                // Add the generated code to the compilation
                context.AddSource("MyClassExtensions", SourceText.From(generatedCode, Encoding.UTF8));
            }
        }
    }
}