using System;
using System.Collections.Generic;
using System.Text;
using CrimsonForthCompiler.Visitors;

namespace CrimsonForthCompiler.Visitors.CrimsonForthVisitor {
    class CFWriter : LanguageWriter {

        StringBuilder buffer = new StringBuilder();

        public override void WriteUnaryArithmeticExpression(string operand) {
            switch (operand) {
                case "-": {
                    this.buffer.Append("NEGATE");
                    break;
                }
                case "~": {
                    this.buffer.Append("INVERT");
                    break;
                }
                case "!": {
                    this.buffer.Append("=0");
                    break;
                }
            }
        }

        public override void WriteBinaryArithmeticExpression(string operand) {
            switch (operand) {
                case "&": {
                    this.buffer.Append("AND");
                    break;
                }
                case "^": {
                    this.buffer.Append("XOR");
                    break;
                }
                case "|": {
                    this.buffer.Append("OR");
                    break;
                }
                case "==": {
                    this.buffer.Append("=");
                    break;
                }
                case "!=": {
                    this.buffer.Append("<>");
                    break;
                }
                case "<=": {
                    this.buffer.Append("<=");
                    break;
                }
                case "<": {
                    this.buffer.Append("<");
                    break;
                }
                case ">": {
                    this.buffer.Append("<=");
                    this.buffer.Append("=0");
                    break;
                }
                case ">=": {
                    this.buffer.Append("<");
                    this.buffer.Append("=0");
                    break;
                }
                case ">>": {
                    this.buffer.Append("RSHIFT");
                    break;
                }
                case "<<": {
                    this.buffer.Append("LSHIFT");
                    break;
                }
                case "+": {
                    this.buffer.Append("+");
                    break;
                }
                case "-": {
                    this.buffer.Append("-");
                    break;
                }
                case "*": {
                    this.buffer.Append("*");
                    break;
                }
                // TODO
                case "/": {
                    break;
                }
                // TODO
                case "%": {
                    break;
                }
            }
        }

    }
}
