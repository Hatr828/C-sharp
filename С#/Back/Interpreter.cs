using System.Net.Http.Headers;
using С_.Front;

namespace С_.Back
{
    public class Interpreter(Parser parser)
    {
        private Dictionary<string, object?> _variables = new Dictionary<string, object?>();

        private Stack<string> _stack = new Stack<string>();

        private LinkedListNode<Node> _current;

        public Parser Parser = parser;

        public string output = "";

        private readonly Dictionary<Operators, Func<object?, object?, object?>> _operationsDict = new()
        {
            { Operators.Plus, Operations.Add },
            { Operators.Mult, Operations.Multiplay },
            { Operators.Div, Operations.Divide },
        };

        public void Execude()
        {
            Parser.Execude();

            _current = Parser.Nodes.First ?? throw new ArgumentNullException("Tokens is null.");

            foreach (var node in Parser.Nodes)
            {
                Eval(node);
            }
        }

        private object? Eval(Node node)
        {       
            if(node == null) return null;

            switch (node)
            {
                case Decl.Var newVariable:
                    {
                        var value = Eval(newVariable.expr);

                        _variables[newVariable.name] = value;

                        return value;
                    }

                case Stmt.Print print:
                    {
                        object? value = Eval(print.expr);

                        output += value.ToString() + "\n";

                        return null;
                    }

                case Expr.LitInt newLitInt: return newLitInt.Value;
                case Expr.LitStr newLitStr: return newLitStr.Value;
                case Expr.LitIdent newLitIdent: return _variables[newLitIdent.Value];

                case Expr.Binary binary:
                    {
                        if(binary.Op != Operators.Equals)
                        {
                            return _operationsDict[binary.Op].Invoke(Eval(binary.L), Eval(binary.R));
                        }

                        if (binary.L.GetType() != typeof(Expr.LitIdent)) throw new ArgumentException("Exepted ident before =");

                        Expr.LitIdent ident = (Expr.LitIdent) binary.L;               

                        _variables[ident.Value] = Eval(binary.R);

                        return null;
                    }

                default: throw new ArgumentException("node not found: " + node);
            }
        }
    }
}
