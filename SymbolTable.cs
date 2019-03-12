using System;
using System.Collections.Generic;
using System.Text;

namespace CrimsonForthCompiler {

    class SymbolTable {

    }

    class Symbol {

        public enum Type {
            VOID,
            INT,
            ERROR,
        }

        public enum Construct {
            FUNCTION,
            STRUCT,
            VARIABLE,
            ARRAY,
            ERROR,
        }

        public readonly string id;
        public readonly Type type;
        public readonly Construct construct;
        public readonly List<Symbol> submembers;
        public readonly uint scope;
        public readonly uint size;

        public Symbol(string id, Type type, Construct construct, uint scope, uint size) {
            this.id = id;
            this.type = type;
            this.construct = construct;
            this.submembers = new List<Symbol>();
            this.scope = scope;
            this.size = size;
        }

        public void AddMembers(Symbol symbol) {
            this.submembers.Add(symbol);
        }

    }

}
