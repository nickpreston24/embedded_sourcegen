using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace fast_enums;

[Generator]
public sealed class ExampleGenerator : IIncrementalGenerator
{
    private static Assembly my_ass = Assembly.GetExecutingAssembly();
    private static Assembly their_ass = Assembly.GetCallingAssembly();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(async ctx =>
        {
            string hell_code = await my_ass.ReadFile("HellWorld.template");
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
                                    Console.WriteLine("Hello From Generator");
                                }
                            }
                        }
                        """;
            ctx.AddSource("ExampleGenerator.g.cs",
                SourceText.From(sourceText, Encoding.UTF8));
        });
    }
}

public static class EmbeddedExtensions
{
    public static async Task<string> ReadFile(
        this Assembly ass,
        string file_hint)
    {
        string resourcePath = ass
            .GetManifestResourceNames()
            .FirstOrDefault(x =>
                x.Contains(file_hint));

        using (Stream stream = ass.GetManifestResourceStream(resourcePath))
        using (StreamReader reader = new StreamReader(stream))
        {
            return await reader.ReadToEndAsync();
        }
    }
}