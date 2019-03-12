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

        public void AddSymbol(Symbol symbol) {
            if (symbol.scope != this.internalScope)
                throw new BadSymbolScopeException(symbol);
            this.symbols.Push(symbol);
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
                while (this.symbols.Count > 0 && this.symbols.Peek().scope == this.internalScope)
                    this.symbols.Pop();
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
