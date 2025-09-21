using System;
using System.Diagnostics;
using System.Xml.Linq;
using static С_.Front.Expr;

namespace С_.Front
{
    public abstract record Node;
    public abstract record Expr : Node
    {
        public sealed record LitInt(long Value) : Expr;
        public sealed record LitStr(string Value) : Expr;
        public sealed record LitIdent(string Name) : Expr;
        public sealed record LitArrayIdent(string Name, int Index) : Expr;
        public sealed record Binary(Operators Op, Expr L, Expr R) : Expr;

    }

    public abstract record Stmt : Node
    {
        public sealed record Print(Expr expr) : Stmt;
    }

    public abstract record Decl : Node
    {
        public sealed record Var(Literals Type, string name, Expr expr) : Decl;
        public sealed record Array(Literals Type, string name, List<Expr> exprs, int Length) : Decl;
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
        Mult,
        Div,
        Equals
    }
    public enum KeyWords
    {
        Int,
        Print,
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
