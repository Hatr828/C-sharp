using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static С_.Front.Decl;

namespace С_.Back
{
    public sealed class Env
    {
        private Dictionary<string, object?> _variables = new();
        private readonly Stack<Log> _log = new();
        private int _depth = 0;

        private readonly struct Log
        {
            public readonly string? Name;     
            public readonly int Depth;

            public Log(string? name, int depth)
            {
                Name = name; Depth = depth;
            }
        }

        public void BeginScope()
        {
            _depth++; 
        }

        public void EndScope()
        {
            while (_log.TryPop(out var entry))
            {
                if (entry.Depth < _depth) { _depth--; return; }

                _variables.Remove(entry.Name);
            }
        }

        public void Set(string name, object? value)
        {
            if (_variables.ContainsKey(name))
            {
                _variables[name] = value;                                  
            }
            else
            {
                _variables[name] = value;

                _log.Push(new Log(name, _depth));
            }
        }

        public object Get(string name) => _variables[name];

    }
}
