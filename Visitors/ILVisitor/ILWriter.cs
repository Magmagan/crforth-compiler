using System;
using System.Collections.Generic;
using System.Text;

namespace CrimsonForthCompiler {
    class ILWriter {

        StringBuilder program;

        bool inFunction = false;
        Stack<StringBuilder> scopedInstructions;
        StringBuilder fullProgram;

        public ILWriter() {
            this.program = new StringBuilder();
            this.scopedInstructions = new Stack<StringBuilder>();
            this.fullProgram = new StringBuilder();
        }

        public void EnterFunction() {
            if (this.inFunction)
                throw new Exception("ILWriter: Attempt to enter function, already in function");
            this.scopedInstructions.Push(new StringBuilder());
            this.inFunction = true;
        }

        public void ExitFunction() {
            if (!this.inFunction)
                throw new Exception("ILWriter: Attempt to exit function, not in function");
            this.inFunction = false;
            this.fullProgram.Append(scopedInstructions.Pop());
        }

        public void EnterWhile() {

        }

        public void ExitWhile() {

        }

        public void WriteArithmeticOperand(string operand) {

            switch (operand) {
                case "||": {
                    // You're Screwed
                    break;
                }
                case "&&": {
                    // You're Screwed
                    break;
                }
                case "&": {
                    this.WriteBitwiseAnd();
                    break;
                }
                case "^": {
                    this.WriteBitwiseXor();
                    break;
                }
                case "|": {
                    this.WriteBitwiseOr();
                    break;
                }
                case "==": {
                    this.WriteLogicalEquality();
                    break;
                }
                case "!=": {
                    this.WriteLogicalEquality();
                    this.WritePush(0);
                    this.WriteLogicalEquality();
                    break;
                }
                case "<=": {
                    this.WriteLogicalGreater();
                    this.WritePush(0);
                    this.WriteLogicalEquality();
                    break;
                }
                case "<": {
                    this.WriteLogicalLesser();
                    break;
                }
                case ">": {
                    this.WriteLogicalGreater();
                    break;
                }
                case ">=": {
                    this.WriteLogicalLesser();
                    this.WritePush(0);
                    this.WriteLogicalEquality();
                    break;
                }
                case ">>": {
                    this.WriteRightShift();
                    break;
                }
                case "<<": {
                    this.WriteLeftShift();
                    break;
                }
                case "+": {
                    this.WriteAdd();
                    break;
                }
                case "-": {
                    this.WriteSubtract();
                    break;
                }
                case "*": {
                    this.WriteMultiply();
                    break;
                }
                case "/": {
                    this.WriteDivide();
                    break;
                }
                case "%": {
                    this.WriteModulo();
                    break;
                }
                default: {
                    throw new Exception($"ILWriter: Invalid operand: #{operand}");
                }
            }

        }

        public void WriteLabel(int labelNumber) {
            this.WriteLabelInstruction(labelNumber);
        }

        public void WriteUnconditionalJump(int labelNumber) {
            this.WriteBranch(labelNumber);
        }

        public void WriteJumpIfTrue(int labelNumber) {
            this.WriteBranchTrue(labelNumber);
        }

        void WriteBitwiseAnd() { // & and s1
            this.WriteInstructionToStackedScope("and", 1);
        }

        void WriteBitwiseOr() { // | or s1
            this.WriteInstructionToStackedScope("or", 1);
        }

        void WriteBitwiseXor() { // ^ xor s1
            this.WriteInstructionToStackedScope("xor", 1);
        }

        void WriteLogicalEquality() { // == ceq s2
            this.WriteInstructionToStackedScope("ceq", 2);
        }

        void WriteLogicalLesser() { // < clt s2
            this.WriteInstructionToStackedScope("clt", 2);
        }

        void WriteLogicalGreater() { // > cgt s2
            this.WriteInstructionToStackedScope("cgt", 2);
        }

        void WriteLeftShift() { // << shl s1
            this.WriteInstructionToStackedScope("shl", 1);
        }

        void WriteRightShift() { // >> shr s1
            this.WriteInstructionToStackedScope("shr", 1);
        }

        void WriteAdd() { // + add s1
            this.WriteInstructionToStackedScope("add", 1);
        }

        void WriteSubtract() { // - sub s1
            this.WriteInstructionToStackedScope("sub", 1);
        }

        void WriteMultiply() { // * mul s1
            this.WriteInstructionToStackedScope("mul", 1);
        }

        void WriteDivide() { // / div s1
            this.WriteInstructionToStackedScope("div", 1);
        }

        void WriteModulo() { // % rem s1
            this.WriteInstructionToStackedScope("rem", 1);
        }

        //////////
        
        void WriteUnaryNegate() { // NEGATE neg s1
            this.WriteInstructionToStackedScope("neg", 1);
        }

        void WriteUnaryBitwiseNot() { // ~ not s1
            this.WriteInstructionToStackedScope("not", 1);
        }

        //////////

        void WriteLabelInstruction(int labelNumber) {
            this.WriteInstructionToStackedScope($"LBL_{labelNumber}", 0);
        }

        void WriteBranch(int labelNumber) {
            this.WriteInstructionToStackedScope($"br.s LBL_{labelNumber}", 2);
        }

        void WriteBranchTrue(int labelNumber) {
            this.WriteInstructionToStackedScope($"brtrue.s LBL_{labelNumber}", 2);
        }

        //////////

        void WritePush(int number) {
            this.WriteInstructionToStackedScope($"ldc.i4 {number}", 2);
        }
        
        void WriteDup() { // DUP dup s1
            this.WriteInstructionToStackedScope("dup", 1);
        }

        void WriteDrop() { // DROP pop s1
            this.WriteInstructionToStackedScope("pop", 1);
        }

        void WriteNop() { // NOP nop s1
            this.WriteInstructionToStackedScope("nop", 1);
        }

        void WriteInstructionToStackedScope(string instruction, int width) {

            if (!this.inFunction) {
                throw new Exception("ILWriter: Attempt to write instruction out of function");
            }
            this.scopedInstructions.Peek().AppendLine($"{width}:  {instruction}");

        }

        public void Dump() {
            Console.WriteLine(this.fullProgram.ToString());
        }

        public void Test() {
            this.EnterFunction();
            this.WriteAdd();
            this.WriteArithmeticOperand("!=");
            this.WriteDrop();
            this.ExitFunction();
            Console.WriteLine(this.scopedInstructions.Peek().ToString());
        }

    }
}
