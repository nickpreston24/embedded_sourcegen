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
public sealed class StoredProcGenerator : IIncrementalGenerator
{
    private static Assembly my_ass = Assembly.GetExecutingAssembly();
    private static Assembly your_ass = Assembly.GetCallingAssembly();

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(async ctx =>
        {
            string source_code = await your_ass.ReadFile("Todos.sql");
            string template_code = await your_ass.ReadFile("sp.template");


            var sp_regex = new Regex(@"DELIMITER\s*
(?<delimiter>[\w^]+)\s*  # get the delimiter name
CREATE\s+PROCEDURE\s+
(?<procedure_name>\w+)\s*  # get the procedure name
\(\s*
(?<parameters>(?<param>.+\s*)*?)  # match any params or none.
\)\s*
BEGIN\s*
(?<code>(?<line>.+\s*)*?)
END\s+(?<delimiter>[\w^]+)\s*
DELIMITER\s*;",
                RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace |
                RegexOptions.IgnoreCase | RegexOptions.Multiline
            ); // https://regex101.com/r/W6Ar1r/1


            var stored_procs = source_code.Extract<StoredProc>(sp_regex);

            string proc_name = stored_procs.FirstOrDefault().procedure_name;


            var map = new Dictionary<string, string>()
            {
                [@"ProcName"] = proc_name
            };

            source_code = template_code
                .Split('\n')
                .ReplaceAll(map)
                .Rollup();

            string hydrated_template =
                template_code.Split('\n').ReplaceAll(map).Rollup();

            ctx.AddSource("StoredProcGenerator.g.cs",
                SourceText.From(hydrated_template, Encoding.UTF8));
        });
    }
}

public class StoredProc
{
    public string delimiter { get; set; } = string.Empty;
    public string procedure_name { get; set; } = string.Empty;
    public string parameters { get; set; } = string.Empty;
    public string code { get; set; } = string.Empty;
}