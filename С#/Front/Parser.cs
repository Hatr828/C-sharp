using System.Diagnostics.Metrics;
using System.Globalization;
using System.Reflection.Metadata.Ecma335;
using static С_.Front.Stmt;

namespace С_.Front
{
    public class Parser(Lexer lexer)
    {
        public Lexer Lexer = lexer;

        private LinkedListNode<Token> _current;

        public LinkedList<Node> Nodes = new LinkedList<Node>();

        public readonly Dictionary<Operators, byte> OperatorPriority = new()
        {
            { Operators.Mult, 70 },
            { Operators.Div, 70},

            { Operators.Plus, 60 },

            { Operators.Greater,        40 }, 
            { Operators.GreaterOrEqual, 40 },  
            { Operators.Less,           40 },  
            { Operators.LessOrEqual,    40 },

            { Operators.Equals,         30 },  
            { Operators.NotEquals,      30 }, 
        };
        public readonly Dictionary<KeyWords, Literals> literalsDict = new()
        {
            { KeyWords.Int, Literals.Int },
            { KeyWords.String, Literals.String },
            { KeyWords.Bool, Literals.Bool },
        };
                                                                
        public void Execute()
        {
            Nodes.Clear();

            Lexer.Execute();

            _current = Lexer.Tokens.First;

            while (_current != null)
            {
                Nodes.AddLast(Eval(Pop()));
            }
        }

        private Node Eval(Token token)
        {
            switch (token.Type)
            {
                case Delimiters del when del == Delimiters.Semicolon:
                    {
                        return null;
                    }

                case KeyWords kw when literalsDict.ContainsKey(kw) && PeekType() is Literals.Ident:
                    {
                        Literals literal = literalsDict[kw];
                        string name = Pop().Val ?? throw new NullReferenceException();

                        if (PeekType() is Delimiters.Semicolon)
                        {
                            Pop();

                            return new Decl.Var(literal, name, GetDefaultValue(literal));
                        }

                        Expect(Operators.Assign, "Operator Assign not fund", true);

                        Expr expr = ParseExpr(0);

                        Expect(Delimiters.Semicolon, "Semicolon expected", true);

                        return new Decl.Var(literal, name, expr);
                    }

                case KeyWords kw when literalsDict.ContainsKey(kw) && PeekType() is Delimiters.LBracket:
                    {
                        Literals type = literalsDict[kw];

                        Pop(); // [
                        Expect(Delimiters.RBracket, "] not found", true);

                        string name = Pop().Val ?? throw new NullReferenceException();

                        if (PeekType() is Delimiters.Semicolon)
                        {
                            Pop();

                            return new Decl.Array(type, name, null, 0); 
                        }

                        Expect(Operators.Assign, "Operator Assign not fund", true);

                        if (PeekType() is KeyWords.New)
                        {
                            Pop(); // new
                            Expect(kw, $"{type} not found", true);
                            Expect(Delimiters.LBracket, "[ not found", true);

                            int length = int.Parse(Pop().Val);

                            Expect(Delimiters.RBracket, "] not found", true);
                            Expect(Delimiters.Semicolon, "Semicolon not found", true);

                            return new Decl.Array(type, name, null, length);

                        }

                        Expect(Delimiters.LBrace, "Unknown array initialization", true);

                        List<Expr> list = new List<Expr>();
                        while (true)
                        {
                            list.Add(ParseExpr(0));
                            if (PeekType() is Delimiters.RBrace) break;

                            Expect(Delimiters.Comma, "Comma not found", true);
                        }

                        Expect(Delimiters.RBrace, "} not found", true);
                        Expect(Delimiters.Semicolon, "Semicolon expected");

                        return new Decl.Array(type, name, list, list.Count);
                    }

                case KeyWords kw when kw == KeyWords.Print:
                    {
                        Expect(Delimiters.LParen, "After print should be (", true);

                        Expr expr = ParseExpr(0);


                        Expect(Delimiters.RParen, "After print should be )", true);
                        Expect(Delimiters.Semicolon, "Semicolon expected", true);

                        return new Stmt.Print(expr);
                    }

                case KeyWords kw when kw == KeyWords.If:
                    {
                        var (cond, block) = ParseParenExprAndOptionalBlock();

                        LinkedList<(Expr, Block)> ifElses = new();

                        while (_current != null && PeekType() is KeyWords.Else && Peek2Type() is KeyWords.If)
                        {
                            Pop(); Pop();
                            ifElses.AddLast(ParseParenExprAndOptionalBlock());

                        }

                        LinkedList<Node> blockElse = new();

                        if(_current != null && PeekType() is KeyWords.Else)
                        {
                            Pop();
                            Expect(Delimiters.LBrace, "Close brace not found", true);

                            while (PeekType() is not Delimiters.RBrace)
                            {
                                blockElse.AddLast(Eval(Pop()));
                            }

                            Pop();
                        }

                        return new Stmt.If(cond, block, ifElses, new Block(blockElse));
                    }

                case KeyWords kw:
                    {
                        return kw switch
                        {
                            KeyWords.True => new Expr.LitBool(true),
                            KeyWords.False => new Expr.LitBool(false),

                            _ => throw new ArgumentException("unknown token: " + token.ToString())
                        };
                    }

                case Literals lit:
                    {
                        Expr left;

                        if (lit == Literals.Int) { left = new Expr.LitInt(int.Parse(token.Val)); }
                        else if (lit == Literals.String) { left = new Expr.LitStr(token.Val); }
                        else if (lit == Literals.Ident) 
                        {
                            if (PeekType() is Delimiters.LBracket)
                            {
                                Pop(); // [
                                left = new Expr.LitArrayIdent(token.Val, int.Parse(Pop().Val));
                                Expect(Delimiters.RBracket, "] not found", true);
                            }
                            else
                            {
                                left = new Expr.LitIdent(token.Val);
                            }
                        }
                        else { throw new ArgumentException("Unknown type: " + token.Type); }

                        if (PeekType() is Operators.Assign)
                        {
                            Operators op = (Operators)Pop().Type;

                            Expr right = ParseExpr(0);

                            return new Expr.Binary(op, left, right);
                        }
                        else
                        {
                            return left;
                        }
                    }

                default:
                    throw new ArgumentException("unknown token: " + token.ToString());
            }
        }

        private Expr ParseExpr(int minBp = 0)
        {
            Expr left = ParsePrimary();

            while (_current != null && _current.Value.Type is Operators)
            {
                Operators op = (Operators)Peek().Type;
                byte pr = OperatorPriority[op];

                if (pr < minBp) break;

                Pop();

                Expr right = ParseExpr(pr + 1);

                left = new Expr.Binary(op, left, right);
            }

            return left;
        }

        private Expr ParsePrimary()
        {
            Token token = Pop();

            if (token.Type is Literals || token.Type is KeyWords.True or KeyWords.False)
                return (Expr)Eval(token);

            if (token.Type is Delimiters.LParen)
            {
                var inner = ParseExpr(0);
                Expect(Delimiters.RParen, ") not found", true);
                return inner;
            }

            throw new ArgumentException("Excepted expr but got: " + token.ToString());
        }



        private Token Peek()
        {
            return _current?.Value ?? throw new NullReferenceException();
        }
        private Enum Peek2Type()
        {
            return _current?.Next?.Value.Type ?? throw new NullReferenceException();
        }
        private Enum PeekType()
        {
            return _current?.Value.Type ?? throw new NullReferenceException();
        }

        private Token Pop()
        {
            if (_current is null)
                throw new InvalidOperationException("No current node");

            var value = _current.Value;
            _current = _current.Next;
            return value;
        }

        private Expr GetDefaultValue(Literals lit)
        {
            if (lit == Literals.Int) return new Expr.LitInt(0);
            if (lit == Literals.Bool) return new Expr.LitBool(false);
            if (lit == Literals.Array) return new Expr.LitStr(null);
            else if (lit == Literals.String) return new Expr.LitStr(null);
            else throw new ArgumentException("Unknow type");
        }
        private void Expect(Enum expect, string msg, bool toPop = false)
        {
            if (toPop)
            {
                if (!Equals(Pop().Type, expect))
                    throw new ArgumentException(msg);
            }
            else
            {
                if (!Equals(PeekType(), expect))
                    throw new ArgumentException(msg);
            }
        }

        private (Expr, Block) ParseParenExprAndOptionalBlock()
        {
            Expect(Delimiters.LParen, "After if should be (", true);

            Expr cond = ParseExpr(0);


            Expect(Delimiters.RParen, "After if should be )", true);

            LinkedList<Node> nodes = new LinkedList<Node>();

            if (PeekType() is Delimiters.LBrace)
            {
                Pop();

                while (PeekType() is not Delimiters.RBrace)
                {
                    nodes.AddLast(Eval(Pop()));
                }

                Pop();
            }

            return (cond, new Block(nodes));
        }
    }
}
