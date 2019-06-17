using System;
using System.Collections.Generic;
using System.Text;

namespace CrimsonForthCompiler.Visitors {
    abstract class LanguageWriter {

        public abstract void WriteExpression(string operand);

    }
}
