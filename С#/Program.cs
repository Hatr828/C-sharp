using System.Diagnostics.Metrics;
using System.Text;
using С_.Back;
using С_.Front;

public static class Program
{
    public static void Main()
    {
        var root = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.Parent!;
        var path = Path.Combine(root.FullName, "Test.txt");
        string code = File.ReadAllText(path, Encoding.UTF8);

        Parser parser = new Parser(new Lexer(code));

        Interpreter interpt = new Interpreter(parser);

        interpt.Execute();

        Console.WriteLine(interpt.output);
        Console.WriteLine(string.Join("\n", parser.Nodes));
    }
}