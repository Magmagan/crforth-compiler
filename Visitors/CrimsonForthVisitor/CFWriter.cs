using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using CrimsonForthCompiler.Visitors;

namespace CrimsonForthCompiler.Visitors.CrimsonForthVisitor {
    class CFWriter : LanguageWriter {

        bool inputWasUsed = false;
        bool outputWasUsed = false;
        bool divisionWasUsed = false;
        bool moduloWasUsed = false;

        bool inGlobalBuffer = false;

        private readonly StringBuilder buffer = new StringBuilder();
        private readonly StringBuilder globalBuffer = new StringBuilder();

        public override void WriteRaw(string assembly) {
            StringBuilder buffer = this.CurrentBuffer();

            buffer.AppendLine(assembly.Replace("$", ""));
        }

        public void WriteProgramSize() {
            StringBuilder buffer = this.CurrentBuffer();

            buffer.AppendLine("PROGRAM_SIZE");
        }

        public void WriteContextRegisterWrite() {
            this.WriteRegisterWrite(1);
        }

        public void WriteContextRegisterRead() {
            this.WriteRegisterRead(1);
        }

        public override void WriteRegisterWrite(int register) {
            StringBuilder buffer = this.CurrentBuffer();

            buffer.AppendLine($"Gr{Convert.ToString(register, 16)}<");
        }

        public override void WriteRegisterRead(int register) {
            StringBuilder buffer = this.CurrentBuffer();

            buffer.AppendLine($"Gr{Convert.ToString(register, 16)}>");
        }

        public override void WriteMemoryWrite() {
            StringBuilder buffer = this.CurrentBuffer();

            buffer.AppendLine("!");
        }

        public override void WriteVariableAddress(string name, int address) {
            StringBuilder buffer = this.CurrentBuffer();

            buffer.Append($"VAR_{name}: ");
            this.WriteImmediate(address);
        }

        public override void WriteMemoryAccess(int address) {
            StringBuilder buffer = this.CurrentBuffer();

            buffer.Append("VAR: ");
            this.WriteImmediate(address);
            buffer.AppendLine("@");
        }

        public override void WriteMemoryAccess() {
            StringBuilder buffer = this.CurrentBuffer();

            buffer.AppendLine("@");
        }

        public override void WriteImmediate(int number) {
            StringBuilder buffer = this.CurrentBuffer();

            buffer.AppendLine($"{Math.Abs(number)}");
            if (number < 0)
                this.WriteUnaryArithmeticExpression("-");
        }

        public override void WriteImmediate(string number) {
            this.WriteImmediate(int.Parse(number));
        }

        public override void WriteFunctionCall(string functionLabel) {
            StringBuilder buffer = this.CurrentBuffer();

            if (functionLabel == "LBL_FN_input")
                this.inputWasUsed = true;
            if (functionLabel == "LBL_FN_output")
                this.outputWasUsed = true;

            this.WriteNoOperation("RSP");
            buffer.AppendLine("PC>");
            this.WriteImmediate(6);
            this.WriteBinaryArithmeticExpression("+");
            this.WriteNoOperation("PSP");
            this.WriteUnconditionalJump(functionLabel);
            this.WriteNoOperation("PSP");
        }

        public override void WriteNoOperation(string stackSelector) {
            StringBuilder buffer = this.CurrentBuffer();

            switch (stackSelector) {
                case "PSP":
                case "RSP": {
                    buffer.AppendLine($"{stackSelector} NOP");
                    break;
                }
                default: {
                    buffer.AppendLine("NOP");
                    break;
                }
            }
        }

        public override void WriteFunctionExit() {
            this.WriteNoOperation("RSP");
            this.WriteUnconditionalJump();
        }

        public override void WriteLabel(string label) {
            StringBuilder buffer = this.CurrentBuffer();

            buffer.Append($"{label}: ");
        }

        public override void WriteUnconditionalJump() {
            StringBuilder buffer = this.CurrentBuffer();

            buffer.AppendLine("JUMP");
        }

        public override void WriteUnconditionalJump(string label) {
            StringBuilder buffer = this.CurrentBuffer();

            buffer.AppendLine(label);
            buffer.AppendLine("JUMP");
        }

        public override void WriteConditionalJump(string label) {
            StringBuilder buffer = this.CurrentBuffer();

            buffer.AppendLine($"{label}");
            buffer.AppendLine("IF");
        }

        public override void WriteUnaryArithmeticExpression(string operand) {
            StringBuilder buffer = this.CurrentBuffer();

            switch (operand) {
                case "-": {
                    buffer.AppendLine("NEGATE");
                    break;
                }
                case "~": {
                    buffer.AppendLine("INVERT");
                    break;
                }
                case "!": {
                    buffer.AppendLine("0=");
                    break;
                }
                case "&": {
                    string bufferText = buffer.ToString();
                    string[] bufferLines = bufferText.Split("\r\n");
                    if (bufferLines[bufferLines.Length - 2] == "@")
                        buffer.Remove(buffer.Length - "@\r\n".Length, "@\r\n".Length);
                    else
                        Console.WriteLine("EEEERRR");
                    break;
                }
            }
        }

        public override void WriteBinaryArithmeticExpression(string operand) {
            StringBuilder buffer = this.CurrentBuffer();

            switch (operand) {
                case "&": {
                    buffer.AppendLine("AND");
                    break;
                }
                case "^": {
                    buffer.AppendLine("XOR");
                    break;
                }
                case "|": {
                    buffer.AppendLine("OR");
                    break;
                }
                case "==": {
                    buffer.AppendLine("=");
                    break;
                }
                case "!=": {
                    buffer.AppendLine("<>");
                    break;
                }
                case "<=": {
                    buffer.AppendLine("<=");
                    break;
                }
                case "<": {
                    buffer.AppendLine("<");
                    break;
                }
                case ">": {
                    buffer.AppendLine("<=");
                    buffer.AppendLine("0=");
                    break;
                }
                case ">=": {
                    buffer.AppendLine("<");
                    buffer.AppendLine("0=");
                    break;
                }
                case ">>": {
                    buffer.AppendLine("RSHIFT");
                    break;
                }
                case "<<": {
                    buffer.AppendLine("LSHIFT");
                    break;
                }
                case "+": {
                    buffer.AppendLine("+");
                    break;
                }
                case "-": {
                    buffer.AppendLine("-");
                    break;
                }
                case "*": {
                    buffer.AppendLine("*");
                    break;
                }
                case "/": {
                    this.divisionWasUsed = true;
                    this.WriteFunctionCall("LBL_FN__divide");
                    break;
                }
                case "%": {
                    this.moduloWasUsed = true;
                    this.WriteFunctionCall("LBL_FN__modulo");
                    break;
                }
            }
        }

        private StringBuilder CurrentBuffer() {
            return this.inGlobalBuffer ? this.globalBuffer : this.buffer;
        }

        public void EnableGlobalBuffer() {
            this.inGlobalBuffer = true;
        }

        public void DisableGlobalBuffer() {
            this.inGlobalBuffer = false;
        }

        private string RemoveUnusedFunctions(string assembly) {
            if (!this.inputWasUsed) {
                assembly = Regex.Replace(assembly, @"(LBL_FN_input:(\s|.)*?)(LBL_FN.*)", "$3", RegexOptions.Multiline);
            }

            if (!this.outputWasUsed) {
                assembly = Regex.Replace(assembly, @"(LBL_FN_output:(\s|.)*?)(LBL_FN.*)", "$3", RegexOptions.Multiline);
            }

            if (!this.divisionWasUsed) {
                assembly = Regex.Replace(assembly, @"(LBL_FN__divide:(\s|.)*?)(LBL_FN.*)", "$3", RegexOptions.Multiline);
            }

            if (!this.moduloWasUsed) {
                assembly = Regex.Replace(assembly, @"(LBL_FN__modulo:(\s|.)*?)(LBL_FN.*)", "$3", RegexOptions.Multiline);
            }

            return assembly;
        }

        private string RemoveEmptyScopes(string assembly) {

            string emptyContextEntry =
                "Gr1>\r\n"
                + "0\r\n"
                + "+\r\n"
                + "Gr1<\r\n";

            string emptyContextExit =
                "Gr1>\r\n"
                + "0\r\n"
                + "-\r\n"
                + "Gr1<\r\n";

            assembly = assembly.Replace(emptyContextEntry, "");
            assembly = assembly.Replace(emptyContextExit, "");

            return assembly;
        }

        private string AddHeaderCode(string assembly) {
            int lineCount = assembly.Split("\r\n").Length;
            return "PSP NOP\r\n"
                + "PROGRAM_SIZE\r\n"
                + "Gr1<\r\n"
                + this.globalBuffer.ToString()
                + "LBL_FN_main\r\n"
                + "JUMP\r\n"
                + assembly;
        }

        public string Finalize() {

            string assembly = this.buffer.ToString();

            assembly = this.RemoveUnusedFunctions(assembly);
            assembly = this.RemoveEmptyScopes(assembly);
            assembly = this.AddHeaderCode(assembly);

            return assembly.Trim();
        }
    }
}
