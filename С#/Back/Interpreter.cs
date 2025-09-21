using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using С_.Front;
using static С_.Front.Expr;

namespace С_.Back
{
    public class Interpreter(Parser parser)
    {
        private Dictionary<string, object?> _variables = new Dictionary<string, object?>();

        public Parser Parser = parser;

        public string output = "";

        private readonly Dictionary<Operators, Func<object?, object?, object?>> _operationsDict = new()
        {
            { Operators.Plus, Operations.Add },
            { Operators.Mult, Operations.Multiplay },
            { Operators.Div, Operations.Divide },
        };

        public void Execute()
        {
            Parser.Execute();

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

                case Expr.LitInt lit: return lit.Value;
                case Expr.LitStr lit: return lit.Value;
                case Expr.LitArrayIdent lit: { 
                        if(_variables[lit.Name] is object[] arr)
                        {
                            return arr[lit.Index];
                        }
                        else { throw new ArgumentException("Excepted array"); }
                    }
                case Expr.LitIdent lit: return _variables[lit.Name];

                case Expr.Binary binary:
                    {
                        if(binary.Op != Operators.Equals)
                        {
                            return _operationsDict[binary.Op].Invoke(Eval(binary.L), Eval(binary.R));
                        }

                        if (binary.L.GetType() != typeof(Expr.LitIdent)) throw new ArgumentException("Excepted ident before =");

                        Expr.LitIdent ident = (Expr.LitIdent) binary.L;               

                        _variables[ident.Name] = Eval(binary.R);

                        return null;
                    }

                case Decl.Array decl:
                    {
                         object[] arr = new object[decl.Length];

                        if (decl.exprs != null) {
                            for (int i = 0; i < decl.Length; i++)
                            {
                                arr[i] = Eval(decl.exprs[i]);
                            }
                        }

                        _variables[decl.name] = arr;

                        return null;
                    }

                default: throw new ArgumentException("node not found: " + node);
            }
        }
    }
}
