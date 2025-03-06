using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace fast_enums;

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