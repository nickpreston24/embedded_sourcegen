using MyNamespace.Generated;

class Program
{
    static void Main(string[] args)
    {
        var myClass = new MyClass();
        myClass.MyMethod();

        // Use the generated extension method
        myClass.PrintInfo();
    }
}   