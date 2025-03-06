using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CodeMechanic.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace embedded_templates;

[Generator]
public sealed class HydroComponentGenerator : IIncrementalGenerator
{
    private static Assembly my_ass = Assembly.GetExecutingAssembly();
    private static Assembly your_ass = Assembly.GetCallingAssembly();

    private static RegexOptions gmix =
        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace |
        RegexOptions.IgnoreCase | RegexOptions.Multiline;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(async ctx =>
        {
            // await Test(ctx);

            await CreateHydroViewFromAlpine(ctx);
        });
    }

    private async Task CreateHydroViewFromAlpine(
        IncrementalGeneratorPostInitializationContext context)
    {
        string ui_template =
            await your_ass.ReadFile("hydro_component_ui.template");

        string codebehind_template =
            await your_ass.ReadFile("hydro_component_codebehind.template");

        if (codebehind_template.IsEmpty()) return;

        string pines_template = await your_ass.ReadFile("HoverCard");

        string temp = codebehind_template
            .Replace(@"$namespace$", "hydro_app")
            .Replace(@"$name$", "Counter2");

        string ui = ui_template
            .Replace(@"$namespace$", "hydro_app")
            .Replace(@"$name$", "Counter2");

//         var regex = new Regex(
//             @"namespace\s+(?<namespace_name>.*?)\.Pages\.Shared\.Components\s*
// {\s*
// public\s+class\s+Hydro(?<component_name>.*?)\s+:\s+HydroView"
//             , gmix); // https://regex101.com/r/cSGfXh/1
//
//         var hydro_view_template = codebehind_template
//             .Extract<HydroTemplate>(regex)
//             .SingleOrDefault();
//
//         string view_name = hydro_view_template.component_name;
//         string view_namespace = hydro_view_template.component_namespace;
//
//         var map = new Dictionary<string, string>()
//         {
//             [@"$name$"] = view_name,
//             [@"$namespace$"] = view_namespace,
//         };
//
//         string updated_pines = pines_template
//             .Split('\n')
//             .ReplaceAll(map)
//             .Rollup();
//
//         string codebehind = codebehind_template
//             .Split('\n')
//             .ReplaceAll(map)
//             .Rollup();

        // string ui = ui_template.Replace(@"$code$", updated_pines);

        context.AddSource("HydroCounter2.g.cshtml.cs",
            SourceText.From(temp, Encoding.UTF8));

        // context.AddSource("HydroHoverCard_UI.html",
        // SourceText.From(ui, Encoding.UTF8));
    }

//     private static async Task Test(
//         IncrementalGeneratorPostInitializationContext context)
//     {
//         string ui_template =
//             await your_ass.ReadFile("hydro_component_ui.template");
//
//         string codebehind_template =
//             await your_ass.ReadFile("hydro_component_codebehind.template");
//
//         string razor_page_ui_template =
//             await your_ass.ReadFile("razor_page_ui.template");
//
//         string razor_page_codebehind_template =
//             await your_ass.ReadFile("razor_page_codebehind.template");
//
//         // NOTE: this regex explicitly requires that a new razor page contain the name 'Hydro' to be hydrolized.
//
//         var razor_ui_regex = new Regex(@"@page\s*
// @model\s+(((?<namespace_name>[$\w]+.?)+)\.)?Hydro(?<model_name>[$\w]+)",
//             gmix
//         ); // https://regex101.com/r/NRRksB/2
//
//
//         var razor_template = ui_template
//             .Extract<RazorTemplate>(razor_ui_regex)
//             .FirstOrDefault();
//
//         string component_name =
//             razor_template.model_name;
//
//         string component_namespace =
//             razor_template.namespace_name;
//
//         var map = new Dictionary<string, string>()
//         {
//             [@"$name$"] = component_name,
//             [@"$namespace$"] = component_namespace,
//         };
//
//         string ui = ui_template
//             .Split('\n')
//             .ReplaceAll(map)
//             .Rollup();
//
//         string codebehind = codebehind_template
//             .Split('\n')
//             .ReplaceAll(map)
//             .Rollup();
//
//         context.AddSource("HydroComponentsGenerator.g.cs",
//             SourceText.From(ui, Encoding.UTF8));
//     }
}

public class RazorTemplate
{
    public string namespace_name { get; set; } = string.Empty;
    public string model_name { get; set; } = string.Empty;
}

public class HydroTemplate
{
    public string component_name { get; set; } = string.Empty;
    public string component_namespace { get; set; } = string.Empty;
}