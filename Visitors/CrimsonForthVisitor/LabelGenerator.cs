using System;
using System.Collections.Generic;
using System.Text;

namespace CrimsonForthCompiler.Visitors.CrimsonForthVisitor {

    class LabelGenerator {

        private int internalCount;
        private int internalIfCount;
        private int internalWhileCount;

        public LabelGenerator() {
            this.internalCount = 0;
            this.internalIfCount = 0;
            this.internalWhileCount = 0;
        }

        public void IncrementIfCount() {
            this.internalIfCount++;
        }

        public void IncrementWhileCount() {
            this.internalWhileCount++;
        }

        public string GenerateGenericLabel() {
            return $"LBL_GENERIC_{this.internalCount++}";
        }

        public string GenerateIfLabel() {
            return $"LBL_IF_{this.internalIfCount}";
        }

        public string GenerateWhileLabel() {
            return $"LBL_WHILE_{this.internalWhileCount}";
        }

        public string GenerateWhileConditionLabel() {
            return $"LBL_WHILECOND_{this.internalWhileCount}";
        }

        public string GenerateFunctionLabel(string functionName) {
            return $"LBL_FN_{functionName}";
        }

    }
}
