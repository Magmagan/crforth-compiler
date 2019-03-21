using System;
using Antlr4.Runtime;

namespace CrimsonForthCompiler.Grammar {

    partial class CMinusLexer {

        public int errors = 0;

        public override void Emit(IToken token) {
            if (token.Type == ErrorChar) {
                this.errors++;
                Console.WriteLine($"L | Line {token.Line}:{token.StartIndex} - Unrecognized token {token.ToString()}");
            }
            base.Emit(token);
        }

    }
}
