using System;
using System.Collections.Generic;
using System.Text;

namespace CrimsonForthCompiler.Visitors.CrimsonForthVisitor {

    class LabelGenerator {

        int internalCount;

        public LabelGenerator() {
            this.internalCount = 0;
        }

        public string GenerateGenericLabel() {
            return $"LBL_GENERIC_{this.internalCount++}";
        }

        public string GenerateIfLabel() {
            return $"LBL_IF_{this.internalCount++}";
        }

        public string GenerateWhileLabel() {
            return $"LBL_WHILE_{this.internalCount++}";
        }

        public string GenerateFunctionLabel(string functionName) {
            return $"LBL_FN_{functionName}";
        }

    }
}
