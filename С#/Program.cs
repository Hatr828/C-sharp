using System.Diagnostics.Metrics;
using System.Text;
using С_.Back;
using С_.Front;

public static class Program
{
    public static void Main()
    {
        var root = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!;
        var path = Path.Combine(root.FullName, "Test.cs");
        string code = File.ReadAllText(path, Encoding.UTF8);

        Parser parser = new Parser(new Lexer(code));

        Interpreter interpt = new Interpreter(parser);

        parser.Execute();

        Console.WriteLine("Lexer: \n");
        Console.WriteLine(string.Join("\n", parser.Lexer.Tokens));
        Console.WriteLine("\nParser: \n");
        Console.WriteLine(string.Join("\n", parser.Nodes));
        Console.WriteLine("\nResult: \n");

        interpt.Execute();

        Console.WriteLine(interpt.output);
    }
}