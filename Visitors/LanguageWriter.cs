using System;
using System.Collections.Generic;
using System.Text;

namespace CrimsonForthCompiler.Visitors {
    abstract class LanguageWriter {

        public abstract void WriteBinaryArithmeticExpression(string operand);

        public abstract void WriteUnaryArithmeticExpression(string operand);

    }
}
