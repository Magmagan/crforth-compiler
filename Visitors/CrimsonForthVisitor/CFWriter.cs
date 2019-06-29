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

        private readonly StringBuilder buffer = new StringBuilder();

        public void WritePreProgram() {
            // PUSH PROGRAM_SIZE
            // R0<
        }

        public override void WriteRaw(string assembly) {
            this.buffer.AppendLine(assembly.Replace("$", ""));
        }

        public void WriteContextRegisterWrite() {
            this.WriteRegisterWrite(1);
        }

        public void WriteContextRegisterRead() {
            this.WriteRegisterRead(1);
        }

        public override void WriteRegisterWrite(int register) {
            this.buffer.AppendLine($"Gr{Convert.ToString(register, 16)}<");
        }

        public override void WriteRegisterRead(int register) {
            this.buffer.AppendLine($"Gr{Convert.ToString(register, 16)}>");
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
            this.buffer.AppendLine($"{Math.Abs(number)}");
            if (number < 0)
                this.WriteUnaryArithmeticExpression("-");
        }

        public override void WriteImmediate(string number) {
            this.WriteImmediate(int.Parse(number));
        }

        public override void WriteFunctionCall(string functionLabel) {
            if (functionLabel == "LBL_FN_input")
                this.inputWasUsed = true;
            if (functionLabel == "LBL_FN_output")
                this.outputWasUsed = true;

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
                    this.buffer.AppendLine($"{stackSelector} NOP");
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
            this.buffer.AppendLine($"{label}");
            this.buffer.AppendLine("IF");
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
                    this.buffer.AppendLine("0=");
                    break;
                }
                case "&": {
                    string bufferText = this.buffer.ToString();
                    string[] bufferLines = bufferText.Split("\r\n");
                    if (bufferLines[bufferLines.Length - 2] == "@")
                        this.buffer.Remove(this.buffer.Length - "@\r\n".Length, "@\r\n".Length);
                    else
                        Console.WriteLine("EEEERRR");
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
                    this.buffer.AppendLine("0=");
                    break;
                }
                case ">=": {
                    this.buffer.AppendLine("<");
                    this.buffer.AppendLine("0=");
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
                + (lineCount + 5) + "\r\n"
                + "Gr1<\r\n"
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
