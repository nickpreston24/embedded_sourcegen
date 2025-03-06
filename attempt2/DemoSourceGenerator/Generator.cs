using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

[Generator]
public class DemoSourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var enumTypes = context.SyntaxProvider
            .CreateSyntaxProvider(CouldBeEnumerationAsync, GetEnumInfoOrNull)
            .Where(type => type is not null)!
            .Collect<DemoEnumInfo>()
            .SelectMany((enumInfos, _) => enumInfos.Distinct());

        var translations = context.AdditionalTextsProvider
            .Where(text => text.Path.EndsWith("translations.json",
                StringComparison.OrdinalIgnoreCase))
            .Select((text, token) => text.GetText(token)?.ToString())
            .Where(text => text is not null)!
            .Collect<string>();

        var generators = context.GetMetadataReferencesProvider()
            .SelectMany(static (reference, _) => TryGetCodeGenerator(reference, out var factory)
                ? ImmutableArray.Create(factory)
                : ImmutableArray<ICodeGenerator>.Empty)
            .Collect();

        context.RegisterSourceOutput(enumTypes.Combine(translations)
                .Combine(generators),
            GenerateCode);
    }

    private static void GenerateCode(
        SourceProductionContext context,
        ((DemoEnumInfo, ImmutableArray<string>), ImmutableArray<ICodeGenerator>) args)
    {
        var ((enumInfo, translationsAsJson), generators) = args;

        if (generators.IsDefaultOrEmpty)
            return;

        var translationsByClassName = GetTranslationsByClassName(context, translationsAsJson);

        foreach (var generator in generators.Distinct())
        {
            if (translationsByClassName is null
                || !translationsByClassName.TryGetValue(enumInfo.Name, out var translations))
            {
                translations = _noTranslations;
            }

            var ns = enumInfo.Namespace is null ? null : $"{enumInfo.Namespace}.";
            var code = generator.Generate(enumInfo, translations);

            if (!String.IsNullOrWhiteSpace(code))
                context.AddSource($"{ns}{enumInfo.Name}{generator.FileHintSuffix}.g.cs", code);
        }
    }


    private static Dictionary<string, IReadOnlyDictionary<string, string>>?
        GetTranslationsByClassName(SourceProductionContext context,
            ImmutableArray<string> translationsAsJson)
    {
        if (translationsAsJson.Length <= 0)
            return null;

        if (translationsAsJson.Length > 1)
        {
            var error = Diagnostic.Create(DemoDiagnosticsDescriptors.MultipleTranslationsFound,
                null);
            context.ReportDiagnostic(error);
        }

        try
        {
            return JsonConvert.DeserializeObject<
                Dictionary<string, IReadOnlyDictionary<string, string>>>(translationsAsJson[0]);
        }
        catch (Exception ex)
        {
            var error = Diagnostic.Create(DemoDiagnosticsDescriptors.TranslationDeserializationError,
                null,
                ex.ToString());
            context.ReportDiagnostic(error);

            return null;
        }
    }
}

public static class DemoDiagnosticsDescriptors
{
    public static readonly DiagnosticDescriptor MultipleTranslationsFound
        = new("DEMO002",
            "Multiple translations found",
            "Multiple translations found",
            "DemoSourceGenerator",
            DiagnosticSeverity.Error,
            true);

    public static readonly DiagnosticDescriptor TranslationDeserializationError
        = new("DEMO003",
            "Translations could not be deserialized",
            "Translations could not be deserialized: {0}",
            "DemoSourceGenerator",
            DiagnosticSeverity.Error,
            true);
}


public interface ICodeGenerator : IEquat	able<ICodeGenerator>
{
    string? FileHintSuffix { get; }

    string Generate(DemoEnumInfo enumInfo);
}