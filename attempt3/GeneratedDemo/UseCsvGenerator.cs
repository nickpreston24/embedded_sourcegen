namespace GeneratedDemo;

class UseCsvGenerator
{
    public static void Run()
    {
        Console.WriteLine("## CARS");
        Cars.All.ToList().ForEach(c => Console.WriteLine($"{c.Brand}\t{c.Model}\t{c.Year}\t{c.Cc}"));
        // Console.WriteLine("\n## PEOPLE");
        // People.All.ToList().ForEach(p => Console.WriteLine($"{p.Name}\t{p.Address}\t{p._11Age}"));
    }
}