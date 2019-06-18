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

        public abstract void WriteMemoryAccess(int address);

        public abstract void WriteMemoryAccess();

        public abstract void WriteVariableAddress(string name, int address);
    }
}
