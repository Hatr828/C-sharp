namespace С_.Front
{
    public class Parser(Lexer lexer)
    {
        public Lexer Lexer = lexer;

        private LinkedListNode<Token> _current;

        public LinkedList<Node> Nodes = new LinkedList<Node>();

        public readonly Dictionary<Operators, byte> OperatorPriority = new()
        {
            { Operators.Plus, 1 },
            { Operators.Mult, 10 },
            { Operators.Div, 10},
        };

        public void Execude()
        {
            Lexer.Execude();

            _current = Lexer.Tokens.First ?? throw new ArgumentNullException("Tokens is null.");

            while (_current != null)
            {
                Nodes.AddLast(Eval(Pop()));
            }
        }

        private Node Eval(Token token)
        {
            switch (token.Type)
            {
                case Delimeters del when del == Delimeters.Semicolon:
                    {
                        return null;
                    }

                case KeyWords kw when kw == KeyWords.Int && Peek().Type is Literals.Ident:
                    {
                        Literals literal = GetLiteral(kw);
                        string name = Pop().Val ?? throw new ArgumentNullException("Val is null");

                        if (Peek().Type.Equals(Delimeters.Semicolon))
                        {
                            Pop();

                            return new Decl.Var(literal, name, GetDefaultValue(literal));
                        }

                        if (!Pop().Type.Equals(Operators.Equals))
                        {
                            throw new ArgumentException("Operator Equals not fund");
                        }

                        Expr expr = ParseExpr(0);

                        if (!Pop().Type.Equals(Delimeters.Semicolon)) throw new ArgumentException("Semicolon expected");

                        return new Decl.Var(literal, name, expr);
                    }

                case KeyWords kw when kw == KeyWords.Print:
                    {
                        if (!Pop().Type.Equals(Delimeters.LParen)) throw new ArgumentException("After print shoudl be (");

                        Expr expr = ParseExpr(0);

                        if (!Pop().Type.Equals(Delimeters.RParen)) throw new ArgumentException("After print shoudl be )");
                        if (!Pop().Type.Equals(Delimeters.Semicolon)) throw new ArgumentException("Semicolon expected");

                        return new Stmt.Print(expr);
                    }

                case Literals lit:
                    {
                        Expr left;

                        if(lit == Literals.Int) { left = new Expr.LitInt(long.Parse(token.Val));}
                        else if (lit == Literals.String) { left = new Expr.LitStr(token.Val); }
                        else if (lit == Literals.Ident) { left = new Expr.LitIdent(token.Val); }
                        else { throw new ArgumentException("Unknown type: " + token.Type);  }

                        if (Peek().Type.Equals(Operators.Equals))
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

            if (token.Type is Literals)
                return (Expr)Eval(token);

            if (token.Type.Equals(Delimeters.LParen))
            {
                var inner = ParseExpr(0);
                if (!Peek().Type.Equals(Delimeters.RParen)) throw new ArgumentException("Exepted ) but got: " + token.ToString());
                Pop();
                return inner;
            }

            throw new ArgumentException("Exepted expr but got: " + token.ToString());
        }



        private Token Peek()
        {
            return _current?.Value ?? throw new ArgumentNullException("_current is null");
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
            else if (lit == Literals.String) return new Expr.LitStr(null);
            else throw new ArgumentException("Unknow literal");
        }

        private Literals GetLiteral(KeyWords kw)
        {
            if (kw == KeyWords.Int) return Literals.Int;
            else throw new ArgumentException("Unknow literal");
        }
    }
}
