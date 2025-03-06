using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator]
public sealed partial class SampleSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var structPovider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (syntax, cancellationToken) =>
                    syntax.IsKind(SyntaxKind.StructDeclaration),
                transform: static (ctx, cancellationToken) =>
                    (TypeDeclarationSyntax)ctx.Node)
            .WithTrackingName(
                "Syntax"); // WithTrackingName allow to record data about the step and access them from the tests

        var assemblyNameProvider = context.CompilationProvider
            .Select(
                (compilation, cancellationToken) => compilation.AssemblyName)
            .WithTrackingName("AssemblyName");

        var valueProvider = structPovider.Combine(assemblyNameProvider);

        context.RegisterSourceOutput(valueProvider, (spc, valueProvider) =>
        {
            (var node, var assemblyName) =
                (valueProvider.Left, valueProvider.Right);
            spc.AddSource(node.Identifier.ValueText + ".cs",
                SourceText.From($"// {node.Identifier.Text} - {assemblyName}",
                    Encoding.UTF8));
        });
    }
}