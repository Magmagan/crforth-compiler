using System;
using Antlr4.Runtime;

namespace CrimsonForthCompiler.Grammar {
    partial class CMinusLexer {

        public override void Emit(IToken token) {
            if (token.Type == ErrorChar)
                Console.WriteLine($"Line{token.Line}:{token.StartIndex} Unrecognized token {token.ToString()}");
            base.Emit(token);
        }

    }
}
