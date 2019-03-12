using System;
using System.Collections.Generic;
using System.Text;

namespace CrimsonForthCompiler {

    class SymbolTable {

        private uint internalScope;
        private readonly Stack<Symbol> symbols;

        public SymbolTable() {
            this.internalScope = 0;
            this.symbols = new Stack<Symbol>();
        }

        public bool AddSymbol(Symbol symbol) {
            if (symbol.scope != this.internalScope)
                throw new BadSymbolScopeException(symbol);
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
            }
            return null;
        }

        public bool HasSymbol (string symbolId) {
            foreach (Symbol symbol in this.symbols) {
                if (symbol.id == symbolId)
                    return true;
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

        public class ExitZeroScopeException : Exception {

            public ExitZeroScopeException() :
                base("Attempted to exit scope zero.") { }

        }

        public class Symbol {

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

            public void ClearMembers() {
                this.submembers.Clear();
            }

            static public Type StringToType(string type) {
                switch (type) {
                    case "int":
                        return Type.INT;
                    case "void":
                        return Type.VOID;
                    default:
                        return Type.ERROR;
                }
            }

            public override string ToString() {
               return $"ID: {this.id}, TYPE: {this.type}, CONSTRUCT: {this.construct}";
            }

        }

    }

}
