internal class Program
{
    static async Task Main(string[] args)
    {
        SourceGeneratorInCSharp.HeckWorld
            .SayHeck(); // 'Hello' in HelloWord.template is replaced with 'Heck'

        SourceGeneratorInCSharp.StoredProcs
            .search_todos(); // The name of an SP in Todos.sql is grabbed, then turned into a public static variable so we can use it in Intellisense.

        // We can, in the future, treat it as a SqlTransaction or build SqlParams automatically.

        // (NOTE: there are many more applications.  The sky is the limit with Embedded Resources + Regex Extraction + Source Generators!)
    }
}