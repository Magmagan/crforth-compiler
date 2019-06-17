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

    }
}
