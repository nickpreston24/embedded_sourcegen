using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace CodeMechanic.RegularExpressions;

public static class RegexExtensions
{
    private static readonly IDictionary<Type, ICollection<PropertyInfo>>
        _propertyCache =
            new Dictionary<Type, ICollection<PropertyInfo>>();

    internal static string[] ReplaceAll(
        this string[] lines,
        Dictionary<string, string> replacementMap
    )
    {
        Dictionary<string, string> map = replacementMap.Aggregate(
            new Dictionary<string, string>(),
            (modified, next) =>
            {
                // Sometimes in JSON \ have to be represented in unicode.  This reverts it.
                string fixedKey = next.Key.Replace("%5C", @"\")
                    .Replace(@"\\", @"\");
                string fixedValue =
                    Regex.Replace(
                        next.Value,
                        @"\""",
                        "'"
                    );

                modified.Add(fixedKey, fixedValue);
                return modified;
            }
        );

        List<string> results = new List<string>();

        foreach (string line in lines)
        {
            string modified = line;
            foreach (KeyValuePair<string, string> replacement in map)
            {
                modified = Regex.Replace(
                    modified,
                    replacement.Key,
                    replacement.Value,
                    RegexOptions.IgnoreCase
                );
            }

            results.Add(modified);
        }

        return results.ToArray();
    }

    internal static List<T> Extract<T>(
        this string text,
        Regex regex,
        bool enforce_exact_match = false,
        bool debug = false
    )
    {
        var collection = new List<T>();

        // If we get no text, throw if we're in devmode (debug == true)
        // If in prod, we want to probably return an empty set.
        if (string.IsNullOrWhiteSpace(text))
            return debug
                ? throw new ArgumentNullException(nameof(text))
                : collection;

        // Get the class properties so we can set values to them.
        var props = _propertyCache.TryGetProperties<T>().ToList();

        // If in prod, we want to probably return an empty set.
        if (props.Count == 0)
            return debug
                ? throw new ArgumentNullException(
                    $"No properties found for type {typeof(T).Name}"
                )
                : collection;

        var errors = new StringBuilder();

        // if (options == RegexOptions.None)
        //     options =
        //         RegexOptions.Compiled
        //         | RegexOptions.IgnoreCase
        //         | RegexOptions.ExplicitCapture
        //         | RegexOptions.Singleline
        //         | RegexOptions.IgnorePatternWhitespace;

        // var regex = new System.Text.RegularExpressions.Regex(regex_pattern, options, TimeSpan.FromMilliseconds(250));

        var matches = regex.Matches(text).Cast<Match>();

        matches.Aggregate(
            collection,
            (list, match) =>
            {
                if (!match.Success)
                {
                    errors.AppendLine(
                        $"No matches found! Could not extract a '{typeof(T).Name}' instance from regex pattern"
                    );

                    errors.AppendLine(text);

                    var missing = props
                        .Select(property => property.Name)
                        .Except(regex.GetGroupNames(),
                            StringComparer.OrdinalIgnoreCase)
                        .ToArray();

                    if (missing.Length > 0)
                    {
                        errors.AppendLine("Properties without a mapped Group:");
                        missing.Aggregate(errors,
                            (result, name) => result.AppendLine(name));
                    }

                    if (errors.Length > 0)
                        //throw new Exception(errors.ToString());
                        Debug.WriteLine(errors.ToString());
                }

                // This rolls up any and all exceptions encountered and rethrows them,
                // if we're trying to go for an absolute, no exceptions matching of Regex Groups to Class Properties:
                if (enforce_exact_match &&
                    match.Groups.Count - 1 != props.Count)
                {
                    errors.AppendLine(
                        $"{MethodBase.GetCurrentMethod().Name}() "
                        + $"WARNING: Number of Matched Groups ({match.Groups.Count}) "
                        + $"does not equal the number of properties for the given class '{typeof(T).Name}'({props.Count})!  "
                        + $"Check the class type and regex pattern for errors and try again."
                    );

                    errors.AppendLine("Values Parsed Successfully:");

                    for (int groupIndex = 1;
                         groupIndex < match.Groups.Count;
                         groupIndex++)
                    {
                        errors.Append($"{match.Groups[groupIndex].Value}\t");
                    }

                    errors.AppendLine();
                    Debug.WriteLine(errors.ToString());
                    //throw new Exception(errors.ToString());
                }

                object instance = Activator.CreateInstance(typeof(T));

                foreach (var property in props)
                {
                    // Get the raw string value that was matched by the Regex for each Group that was captured:
                    string value = GetValueFromMatch<T>(regex, match, property);

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        property.SetValue(
                            instance,
                            value: TypeDescriptor
                                .GetConverter(property.PropertyType)
                                .ConvertFrom(value),
                            index: null
                        );
                    }
                    else if (property.CanWrite)
                    {
                        property?.SetValue(instance, value: null, index: null);
                    }
                }

                list.Add((T)instance);
                return list;
            }
        );

        return collection;
    }

    private static string GetValueFromMatch<T>(
        Regex regex
        , Match match
        , PropertyInfo property)
    {
#if NETSTANDARD2_0_OR_GREATER
        var group_names = regex.GetGroupNames();
        Group grp = match.Groups[property.Name];
        return grp?.Value.Trim();
#else
        return match
            .Groups.Cast<Group>()
            .SingleOrDefault(group =>
                group.Name.Equals(property.Name,
                    StringComparison.OrdinalIgnoreCase)
            )
            ?.Value.Trim();
#endif
    }
}