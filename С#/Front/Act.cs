using System;
using System.Xml.Linq;

namespace С_.Front
{
    public abstract record Node;
    public abstract record Expr : Node
    {
        public sealed record LitInt(long Value) : Expr;
        public sealed record LitStr(string Value) : Expr;
        public sealed record LitIdent(string Value) : Expr;
        public sealed record Binary(Operators Op, Expr L, Expr R) : Expr;

    }

    public abstract record Stmt : Node
    {
        public sealed record Print(Expr expr) : Stmt;
    }

    public abstract record Decl : Node
    {
        public sealed record Var(Literals Type, string name, Expr expr) : Decl;
    }


    public struct Token
    {
        public required Enum Type;

        public string? Val;

        public override readonly string ToString() => Type switch
        {
            Operators op => $"Op({op})",

            KeyWords kw => $"Kw({kw})",

            _ => $"{Type}{(Val is null ? "" : $"({Val})")}"
        };
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
        Print
    }
    public enum Delimeters
    {
        Semicolon,
        LParen, RParen,
    }

    public enum Literals
    {
        String,
        Int,
        Ident,
    }

    public enum TokenType
    {
        Operator, 
        Keyword,
        Delimeter,
        Eof
    }
}
