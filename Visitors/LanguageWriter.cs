using System;
using System.Collections.Generic;
using System.Text;

namespace CrimsonForthCompiler.Visitors {
    abstract class LanguageWriter {

        public abstract void WriteBinaryArithmeticExpression(string operand);

        public abstract void WriteUnaryArithmeticExpression(string operand);

        public abstract void WriteUnconditionalJump();

        public abstract void WriteUnconditionalJump(string label);

        public abstract void WriteConditionalJump(string label);

        public abstract void WriteLabel(string label);

        public abstract void WriteFunctionExit();

        public abstract void WriteNoOperation(string stackSelector);

        public abstract void WriteFunctionCall(string functionLabel);

        public abstract void WriteImmediate(string number);

        public abstract void WriteImmediate(int number);

        public void ThrowCompilerException(string exception) {
            throw new CompilerException(exception);
        }

        public class CompilerException : InvalidOperationException {
            public CompilerException() : base() { }
            public CompilerException (string exception) : base (exception) { }
            public CompilerException (string exception, Exception innerException) : base(exception, innerException) { }
        }

    }
}
