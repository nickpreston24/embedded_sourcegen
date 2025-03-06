// See https://aka.ms/new-console-template for more information

Console.WriteLine("Hello, World!");

public class Demonstration
{
    public Demonstration()
    {
        SourceGeneratorInCSharp.HeckWorld
            .SayHeck(); // 'Hello' in HelloWord.template is replaced with 'Heck'

        SourceGeneratorInCSharp.StoredProcs
            .search_todos(); // The name of an SP in Todos.sql is grabbed, then turned into a public static variable so we can use it in Intellisense.
        // We can also treat it as a SqlTransaction and build SqlParams automatically.

        // NOTE: there are many more applications.  The sky is the limit with Embedded Resources + Regex Extraction + Source Generators!
    }
}