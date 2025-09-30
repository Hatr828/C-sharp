using System;
using System.Diagnostics;
using System.Xml.Linq;
using static С_.Front.Expr;

namespace С_.Front
{
    public abstract record Node;
    public sealed record Block(List<Node> nodes) : Node;
    public abstract record Expr : Node
    {
        public sealed record LitInt(int Value) : Expr;
        public sealed record LitBool(bool Value) : Expr;
        public sealed record LitStr(string Value) : Expr;
        public sealed record LitIdent(string Name) : Expr;
        public sealed record LitArrayIdent(string Name, int Index) : Expr;
        public sealed record LitFuncCall(string Name, List<Expr> Values) : Expr;
        public sealed record Binary(Operators Op, Expr L, Expr R) : Expr;

    }

    public abstract record Stmt : Node
    {
        public sealed record Print(Expr expr) : Stmt;
        public sealed record While(Expr Cond, Block Block) : Stmt;
        public sealed record If(Expr Cond, Block Block, List<(Expr expr, Block block)>? IfElses = null, Block? Else = null) : Stmt;

    }

    public abstract record Decl : Node
    {
        public sealed record Var(Literals Type, string name, Expr expr) : Decl;
        public sealed record Array(Literals Type, string name, List<Expr> exprs, int Length) : Decl;   
        public sealed record Func(Literals ReturnType, string name, List<(Literals, string)>? Args, Block Block) : Decl;
    }

    [DebuggerDisplay("{Dbg,nq}")]
    public struct Token
    {
        public required Enum Type;

        public string? Val;

        public override readonly string ToString() => Type switch
        {
            Operators op => $"Op({op})",
            KeyWords kw => $"Kw({kw})",
            Delimiters d => $"Delim({d})",
            null => "<uninitialized>",
            _ => $"{Type}{(Val is null ? "" : $"({Val})")}"
        };
        private string Dbg => ToString();
    }

    public enum Operators {
        Plus,
        Minus,
        Mult,
        Div,
        Assign,
        Equals, NotEquals,
        Less, LessOrEqual, Greater, GreaterOrEqual,
    }
    public enum KeyWords
    {
        Print,
        String, Bool, Int, Void, Long,
        True, False,
        If, Else,
        While,
        New
    }
    public enum Delimiters
    {
        Semicolon,
        LParen, RParen,
        LBracket, RBracket,
        LBrace, RBrace,
        Comma
    }

    public enum Literals
    {
        String,
        Bool,
        Long,
        Void,
        Int,
        Ident,
        Array,
    }

    public enum TokenType
    {
        Operator, 
        Keyword,
        Delimeter,
        Eof
    }
}
