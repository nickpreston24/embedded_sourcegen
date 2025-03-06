using System.Linq;
using System.Text;

namespace fast_enums;

public static class StringExtensions
{
    public static string Rollup(this string[] lines_of_code)
    {
        return lines_of_code.Aggregate(
                new StringBuilder(),
                (sb, next) =>
                {
                    sb.AppendLine(next);
                    return sb;
                })
            .ToString();
    }
}