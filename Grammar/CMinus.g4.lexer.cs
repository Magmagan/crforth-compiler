using System;
using Antlr4.Runtime;

namespace CrimsonForthCompiler.Grammar {

    partial class CMinusLexer {

        public int errors = 0;

        public override void Emit(IToken token) {
            if (token.Type == ErrorChar) {
                this.errors++;
                Console.WriteLine($"Lex | Line {token.Line}:{token.Column} - Unrecognized token {token.ToString()}");
            }
            base.Emit(token);
        }

    }
}
