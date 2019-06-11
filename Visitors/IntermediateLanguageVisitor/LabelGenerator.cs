using System;
using System.Collections.Generic;
using System.Text;

namespace CrimsonForthCompiler.Visitors.IntermediateLanguageVisitor {
    class LabelGenerator {

        int internalCount;

        public LabelGenerator() {
            this.internalCount = 0;
        }

        public int GenerateLabel() {
            return this.internalCount++;
        }

    }
}
