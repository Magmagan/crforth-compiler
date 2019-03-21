using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace CrimsonForthCompiler.Grammar {

    partial class CMinusParser {
    }

    public class SyntaxErrorListener : BaseErrorListener {

        //public static readonly SyntaxErrorListener INSTANCE = new SyntaxErrorListener();

        public int errors = 0;
        
        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e) {
            Console.WriteLine($"S | Line {line}:{charPositionInLine} - Unexpected token {offendingSymbol}");
            errors++;
            base.SyntaxError(recognizer, offendingSymbol, line, charPositionInLine, msg, e);
        }

    }

}
