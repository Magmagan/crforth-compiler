using System;
using System.Collections.Generic;
using System.Text;

namespace CrimsonForthCompiler.Visitors.CrimsonForthVisitor {

    class LabelGenerator {

        private int internalCount;
        private int internalIfCount;
        private int internalWhileCount;
        private string internalCurrentFunction;

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

        public string GenerateElseLabel() {
            return $"LBL_IF_ELSE_{this.internalIfCount}";
        }

        public string GenerateWhileLabel() {
            return $"LBL_WHILE_{this.internalWhileCount}";
        }

        public string GenerateWhileConditionLabel() {
            return $"LBL_WHILECOND_{this.internalWhileCount}";
        }

        public string GenerateFunctionLabel(string functionName) {
            this.internalCurrentFunction = functionName;
            return $"LBL_FN_{functionName}";
        }

        public string FunctionReturnLabel() {
            return $"LBL_RETURN_{this.internalCurrentFunction}";
        }

    }
}
