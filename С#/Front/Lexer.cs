using System.Globalization;
using System.Text;

namespace С_.Front
{
    public class Lexer
    {
        public LinkedList<Token> Tokens = new LinkedList<Token>();

        private string _code;

        private int _current = 0;

        public readonly Dictionary<string, Operators> OperatorDict = new()
        {
            { "+", Operators.Plus },
            { "*", Operators.Mult},
            { "/", Operators.Div},
            { "=", Operators.Assign },
            { "==", Operators.Equals },
            { "!=", Operators.NotEquals },
            { ">", Operators.Greater },
            { ">=", Operators.GreaterOrEqual },
            { "<", Operators.Less },
            { "<=", Operators.LessOrEqual },
        };

        public readonly Dictionary<string, Delimiters> DelimiterDict = new()
        {
            { ";", Delimiters.Semicolon },
            { "(", Delimiters.LParen },
            { ")", Delimiters.RParen },
            { "[", Delimiters.LBracket },
            { "]", Delimiters.RBracket },
            { "{", Delimiters.LBrace },
            { "}", Delimiters.RBrace },
            { ",", Delimiters.Comma },
        };

        public readonly Dictionary<string, KeyWords> KeyWordDict = new()
        {
            { "int", KeyWords.Int },
            { "true", KeyWords.True },
            { "bool", KeyWords.Bool },
            { "false", KeyWords.False },
            { "string", KeyWords.String },
            { "new", KeyWords.New },
            { "if", KeyWords.If },
            { "else", KeyWords.Else },
            { "print", KeyWords.Print}
        };

        private static readonly HashSet<char> SingleDelims = new("(){}[];,");
        private static readonly HashSet<char> OpChars = new("+-*/=<>!&|^%");
        private static readonly HashSet<string> TwoCharOps = new() { "==", "!=", "<=", ">=", "&&", "||" };
        private static readonly HashSet<char> Ws = new(" \t\r\n");


        public Lexer(string code)
        {
            _code = code;
        }

        public void Clear()
        {
            Tokens.Clear();
            _current = 0;
        }

        public void Execute()
        {
            Clear();

            while (_current < _code.Length)
            {
                if (_code[_current] is ' ' or '\n' or '\r') { _current++; }
                else
                {
                    Tokens.AddLast(
                        Match(getNext())
                    );
                }
            }
        }

        private Token Match(string str)
        {
            return str switch
            {
                _ when OperatorDict.TryGetValue(str, out var op) => new Token { Type = op },

                _ when KeyWordDict.TryGetValue(str, out var op) => new Token { Type = op },

                _ when DelimiterDict.TryGetValue(str, out var op) => new Token { Type = op },

                _ when int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out var n) => new Token { Type = Literals.Int, Val = str },

                _ when str.Length >= 2 && str[0] == '"' && str[^1] == '"' => new Token { Type = Literals.String, Val = str[1..^1] },

                _ when IsIdent(str) => new Token { Type = Literals.Ident, Val = str },

                _ => throw new ArgumentException("unknown token: " + str)
            };
        }

        private static bool IsIdent(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;
            if (!(char.IsLetter(s[0]) || s[0] == '_')) return false;
            for (int i = 1; i < s.Length; i++)
                if (!(char.IsLetterOrDigit(s[i]) || s[i] == '_')) return false;
            return true;
        }

        private void SkipWs()
        {
            while (_current < _code.Length && Ws.Contains(_code[_current])) _current++;
        }

        private string getNext()
        {
            SkipWs();
            if (_current >= _code.Length) return "";

            int start = _current;
            char c = _code[_current];

            if (_current + 1 < _code.Length)
            {
                var two = _code.AsSpan(_current, 2).ToString();
                if (TwoCharOps.Contains(two))
                {
                    _current += 2;
                    return two;
                }
            }

            if (SingleDelims.Contains(c))
            {
                _current++;
                return c.ToString();
            }

            if (OpChars.Contains(c))
            {
                _current++;
                return c.ToString();
            }

            while (_current < _code.Length
                   && !Ws.Contains(_code[_current])
                   && !SingleDelims.Contains(_code[_current])
                   && !OpChars.Contains(_code[_current]))
            {
                _current++;
            }

            return _code[start.._current];
        }

    }
}
