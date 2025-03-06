using System.Text.RegularExpressions;

namespace embedded_templates;

public struct RegexEnumStruct
{
    public RegexEnumStruct(
        string id
        , string name
        , string pattern,
        string url = "")
    {
        this.id = id;
        this.name = name;
        this.pattern = pattern;
        this.url = url;
        compiled_regex =
            string.IsNullOrWhiteSpace(pattern)
                ? null
                : new Regex(
                    pattern,
                    RegexOptions.Compiled
                    | RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.IgnorePatternWhitespace
                );
    }

    public string id { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;

    public string url { get; set; } = string.Empty;

    public Regex compiled_regex { get; set; } = null;
    // new Regex(@"\w+");
    // // would this work?

    public string pattern { get; set; } = string.Empty;
}