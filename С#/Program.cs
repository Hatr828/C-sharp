using System.Diagnostics;
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

        var sw = Stopwatch.StartNew();
        interpt.Execute();

        sw.Stop();
        Console.WriteLine(interpt.output);
        Console.WriteLine($"Время: {sw.Elapsed}");
        Console.ReadLine();
    }
}