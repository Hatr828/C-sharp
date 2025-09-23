using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using С_.Front;
using static С_.Front.Expr;
using static С_.Front.Stmt;

namespace С_.Back
{
    public class Interpreter(Parser parser)
    {
        private Dictionary<ulong, object?> _variables = new();
        private Dictionary<string, (List<(Literals, string)>? Args, Block Block)> _functions = new();

        public Parser Parser = parser;

        public string output = "";

        private readonly Dictionary<Operators, Func<object?, object?, object?>> _operationsDict = new()
        {
            { Operators.Plus, Operations.Add },
            { Operators.Mult, Operations.Multiplay },
            { Operators.Div, Operations.Divide },

            { Operators.Equals, (a, b) => a.Equals(b) },
            { Operators.NotEquals, (a, b) => !a.Equals(b) },

            { Operators.GreaterOrEqual, Operations.GreaterOrEqual },
            { Operators.Greater, Operations.Greater },
            { Operators.Less, Operations.Less },
            { Operators.LessOrEqual, Operations.LessOrEqual },
        };

        public readonly Dictionary<Literals, Type> typesDict = new()
        {
            { Literals.Int, typeof(int) },
            { Literals.String, typeof(string) },
        };

        public void Clear()
        {
            _variables.Clear();
            output = "";
        }

        public void Execute()
        {
            Clear();

            Parser.Execute();

            foreach (var node in Parser.Prolog)
            {
                Eval(node);
            }

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
                case Expr.LitBool lit: return lit.Value;
                case Expr.LitArrayIdent lit: { 
                        if(_variables[lit.Name] is object[] arr)
                        {                               
                            return arr[lit.Index];
                        }
                        else { throw new ArgumentException("Excepted array"); }
                    }
                case Expr.LitIdent lit: return _variables[lit.Name];
                case Expr.LitFuncCall lit:
                    {
                        /*
                        var (args, block) = _functions[lit.Name];
                        foreach (var ((type, name), value) in args.Zip(lit.Values))
                        {
                            var temp = Eval(value);
                            if (temp.GetType() != typesDict[type]) throw new ArgumentException($"Unsuitable types: { value.GetType() } and { typesDict[type] }");
                            _variables[name] = temp;
                        }

                        foreach (var nod in block.nodes)
                        {
                            Eval(nod);
                        }

                        
                        */
                        return null;
                    }

                case Expr.Binary binary:
                    {
                        if(binary.Op != Operators.Assign)
                        {
                            return _operationsDict[binary.Op].Invoke(Eval(binary.L), Eval(binary.R));
                        }

                        if (binary.L.GetType() != typeof(Expr.LitIdent)) throw new ArgumentException("Excepted ident before =");

                        Expr.LitIdent ident = (Expr.LitIdent) binary.L;               

                        _variables[ident.Name] = Eval(binary.R);

                        return null;
                    }

                case Decl.Func func:
                    {
                        _functions[func.name] = (func.Args, func.Block);

                        return null;
                    }

                case Decl.Array decl:
                    {
                         object[] arr = new object[decl.Length];

                        if (decl.exprs != null) {
                            int i = 0;
                            foreach (Expr expr in decl.exprs)
                            {
                                var temp = Eval(expr);
                                if(temp.GetType() != typesDict[decl.Type])
                                {
                                    throw new ArgumentException($"Cannot convert {temp.GetType()} to {typesDict[decl.Type]}");
                                }
                                arr[i++] = temp;  
                            }
                        }

                        _variables[decl.name] = arr;  

                        return null;
                    }

                case Stmt.If If:
                    {
                        if((bool)Eval(If.Cond))
                        {
                            foreach (var nod in If.Block.nodes)
                            {
                                Eval(nod);
                            }
                        }
                        else
                        {
                            bool useElse = true;
                            foreach ((Expr cond, Block block) in If.IfElses)  
                            {
                                if ((bool)Eval(cond))
                                {
                                    useElse = false;
                                    foreach (var nod in block.nodes)
                                    {
                                        Eval(nod);
                                    }
                                    break;
                                }
                            }

                            if(useElse)
                            {
                                foreach (var nod in If.Else.nodes)
                                {
                                    Eval(nod);
                                }
                            }
                            
                        }
                        
                        return null;
                    }

                case Stmt.While While:
                    {
                        while ((bool)Eval(While.Cond))
                        {
                            foreach (var nod in While.Block.nodes)
                            {
                                Eval(nod);
                            }
                        }

                        return null;
                    }

                default: throw new ArgumentException("node not found: " + node);
            }
        }
    }
}
