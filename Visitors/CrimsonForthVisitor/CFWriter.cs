using System;
using System.Collections.Generic;
using System.Text;
using CrimsonForthCompiler.Visitors;

namespace CrimsonForthCompiler.Visitors.CrimsonForthVisitor {
    class CFWriter : LanguageWriter {

        bool moduloWasUsed = false;
        bool divisionWasUsed = false;
        private readonly StringBuilder buffer = new StringBuilder();

        public void WriteModuloFunction() {
            // : nminusm ( n m -- n-m m ) SWAP OVER - SWAP ;
            // : modulorecursive OVER OVER < 0= IF nminusm recurse THEN ;
            // : modulo ( n m -- n%m ) modulorecursive DROP ;
            // 10 4 modulo ( expect 2 on stack )
        }

        public override void WriteMemoryWrite() {
            this.buffer.AppendLine("!");
        }

        public override void WriteVariableAddress(string name, int address) {
            this.buffer.Append($"VAR_{name}: ");
            this.WriteImmediate(address);
        }

        public override void WriteMemoryAccess(int address) {
            this.buffer.Append("VAR: ");
            this.WriteImmediate(address);
            this.buffer.AppendLine("@");
        }

        public override void WriteMemoryAccess() {
            this.buffer.AppendLine("@");
        }

        public override void WriteImmediate(int number) {
            this.buffer.AppendLine($"{number}");
        }

        public override void WriteImmediate(string number) {
            this.buffer.AppendLine($"{int.Parse(number)}");
        }

        public override void WriteFunctionCall(string functionLabel) {
            this.WriteNoOperation("RSP");
            this.buffer.AppendLine("PC>");
            this.WriteImmediate(6);
            this.WriteBinaryArithmeticExpression("+");
            this.WriteNoOperation("PSP");
            this.WriteUnconditionalJump(functionLabel);
            this.WriteNoOperation("PSP");
        }

        public override void WriteNoOperation(string stackSelector) {
            switch(stackSelector) {
                case "PSP":
                case "RSP": {
                    this.buffer.AppendLine($"NOP {stackSelector}");
                    break;
                }
                default: {
                    this.buffer.AppendLine("NOP");
                    break;
                }
            }
        }

        public override void WriteFunctionExit() {
            this.WriteNoOperation("RSP");
            this.WriteUnconditionalJump();
        }

        public override void WriteLabel(string label) {
            this.buffer.Append($"{label}: ");
        }

        public override void WriteUnconditionalJump() {
            this.buffer.AppendLine("JUMP");
        }

        public override void WriteUnconditionalJump(string label) {
            this.buffer.AppendLine(label);
            this.buffer.AppendLine("JUMP");
        }

        public override void WriteConditionalJump(string label) {
            this.buffer.AppendLine($"IF {label}");
        }

        public override void WriteUnaryArithmeticExpression(string operand) {
            switch (operand) {
                case "-": {
                    this.buffer.AppendLine("NEGATE");
                    break;
                }
                case "~": {
                    this.buffer.AppendLine("INVERT");
                    break;
                }
                case "!": {
                    this.buffer.AppendLine("=0");
                    break;
                }
            }
        }

        public override void WriteBinaryArithmeticExpression(string operand) {
            switch (operand) {
                case "&": {
                    this.buffer.AppendLine("AND");
                    break;
                }
                case "^": {
                    this.buffer.AppendLine("XOR");
                    break;
                }
                case "|": {
                    this.buffer.AppendLine("OR");
                    break;
                }
                case "==": {
                    this.buffer.AppendLine("=");
                    break;
                }
                case "!=": {
                    this.buffer.AppendLine("<>");
                    break;
                }
                case "<=": {
                    this.buffer.AppendLine("<=");
                    break;
                }
                case "<": {
                    this.buffer.AppendLine("<");
                    break;
                }
                case ">": {
                    this.buffer.AppendLine("<=");
                    this.buffer.AppendLine("=0");
                    break;
                }
                case ">=": {
                    this.buffer.AppendLine("<");
                    this.buffer.AppendLine("=0");
                    break;
                }
                case ">>": {
                    this.buffer.AppendLine("RSHIFT");
                    break;
                }
                case "<<": {
                    this.buffer.AppendLine("LSHIFT");
                    break;
                }
                case "+": {
                    this.buffer.AppendLine("+");
                    break;
                }
                case "-": {
                    this.buffer.AppendLine("-");
                    break;
                }
                case "*": {
                    this.buffer.AppendLine("*");
                    break;
                }
                // TODO
                case "/": {
                    this.divisionWasUsed = true;
                    break;
                }
                // TODO
                case "%": {
                    this.moduloWasUsed = true;
                    break;
                }
            }
        }

        public string Finalize() {
            if (this.moduloWasUsed) {

            }

            if (this.divisionWasUsed) {

            }

            return this.DumpBuffer();
        }

        public string DumpBuffer() {
            return this.buffer.ToString();
        }
    }
}
