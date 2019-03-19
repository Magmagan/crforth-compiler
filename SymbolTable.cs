using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CrimsonForthCompiler {

    class SymbolTable {

        private uint internalScope;
        private readonly Stack<Symbol> symbols;
        private readonly List<string> symbolTypes = new List<string>{
            "int",
            "void"
        };

        public SymbolTable() {
            this.internalScope = 0;
            this.symbols = new Stack<Symbol>();
        }

        public void AddSymbolType(string symbolType) {
            this.symbolTypes.Add(symbolType);
        }

        public bool AddSymbol(Symbol symbol) {

            if (symbol.scope != this.internalScope)
                throw new BadSymbolScopeException(symbol);

            if (!this.symbolTypes.Contains(symbol.type)) {
                throw new BadSymbolTypeException(symbol);
            }

            if (!this.HasSymbol(symbol.id)) {
                this.symbols.Push(symbol);
                return true;
            }
            else {
                return false;
            }
        }

        public Symbol GetSymbol (string symbolId) {
            foreach (Symbol symbol in this.symbols) {
                if (symbol.id == symbolId)
                    return symbol;
                foreach (Symbol internalSymbol in symbol.submembers) {
                    if (internalSymbol.id == symbolId)
                        return internalSymbol;
                }
            }
            return null;
        }

        public bool HasSymbol (string symbolId) {
            foreach (Symbol symbol in this.symbols) {
                if (symbol.id == symbolId)
                    return true;
                foreach (Symbol internalSymbol in symbol.submembers) {
                    if (internalSymbol.id == symbolId)
                        return true;
                }
            }
            return false;
        }

        public void EnterScope() {
            this.internalScope++;
        }

        public void ExitScope() {

            if(this.internalScope == 0) {
                throw new ExitZeroScopeException();
            }
            else {
                while (this.symbols.Count > 0 && this.symbols.Peek().scope == this.internalScope) {
                    this.symbols.Peek().ClearMembers();
                    this.symbols.Pop();
                }
                this.internalScope--;
            }

        }

        public class BadSymbolScopeException : Exception {

            public BadSymbolScopeException(Symbol symbol) :
                base($"'{symbol.id}' symbol scope not equal to symbol table scope.") { }

        }

        public class BadSymbolTypeException : Exception {

            public BadSymbolTypeException(Symbol symbol) :
                base($"'{symbol.id}' symbol type not in symbol table dictionary.") { }

        }

        public class ExitZeroScopeException : Exception {

            public ExitZeroScopeException() :
                base("Attempted to exit scope zero.") { }

        }

        public class Symbol {

            public enum Construct {
                FUNCTION,
                STRUCT,
                VARIABLE,
                ARRAY,
                ERROR,
            }

            public readonly string id;
            public readonly string type;
            public readonly Construct construct;
            public readonly List<Symbol> submembers = new List<Symbol>();
            public readonly uint scope;
            public readonly uint size;
            public readonly uint pointerCount;

            public Symbol(string id, string type, Construct construct, uint scope, uint size, uint pointerCount) {
                this.id = id;
                this.type = type;
                this.construct = construct;
                this.submembers = new List<Symbol>();
                this.scope = scope;
                this.size = size;
                this.pointerCount = pointerCount;
            }

            public void AddMembers(List<Symbol> symbols) {
                foreach (Symbol symbol in symbols) {
                    this.AddMember(symbol);
                }
            }

            public void AddMember(Symbol symbol) {
                this.submembers.Add(symbol);
            }

            public void ClearMembers() {
                this.submembers.Clear();
            }

            static public string RemoveExtras(string type) {
                return type.Replace("*", "").Replace("struct", "").Replace(" ", "");
            }

            static public uint CountStringAsterisks(string type) {
                return (uint) type.Count(x => x == '*');
            }

            public override string ToString() {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.AppendLine($"ID: {this.id}, TYPE: {this.type}, CONSTRUCT: {this.construct}");
                foreach (Symbol internalSymbol in this.submembers) {
                    stringBuilder.AppendLine($"\tID: {internalSymbol.id}, TYPE: {internalSymbol.type}, CONSTRUCT: {internalSymbol.construct}");
                }
                return stringBuilder.ToString();
            }

        }

    }

}
