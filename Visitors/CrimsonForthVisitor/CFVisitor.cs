using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CrimsonForthCompiler.Grammar;

namespace CrimsonForthCompiler.Visitors.CrimsonForthVisitor {

    class CrimsonForthVisitor : CMinusBaseVisitor<object> {

        public readonly CFWriter writer = new CFWriter();
        private readonly LabelGenerator labelGenerator = new LabelGenerator();
        private readonly MiniSymbolTable symbolTable = new MiniSymbolTable();
        private int inExpression = 0;

        public override object VisitProgram([NotNull] CMinusParser.ProgramContext context) {
            this.symbolTable.EnterContext();
            this.Visit(context.declarationList());
            this.symbolTable.ExitContext();
            return null;
        }

        #region Struct Rules

        // TODO
        public override object VisitStructDeclaration([NotNull] CMinusParser.StructDeclarationContext context) {
            return base.VisitStructDeclaration(context);
        }

        // TODO
        public override object VisitStructVariableDeclaration_Array([NotNull] CMinusParser.StructVariableDeclaration_ArrayContext context) {
            return base.VisitStructVariableDeclaration_Array(context);
        }

        // TODO
        public override object VisitStructVariableDeclaration_Variable([NotNull] CMinusParser.StructVariableDeclaration_VariableContext context) {
            return base.VisitStructVariableDeclaration_Variable(context);
        }

        // TODO
        public override object VisitVariable_StructAccess([NotNull] CMinusParser.Variable_StructAccessContext context) {
            return base.VisitVariable_StructAccess(context);
        }

        #endregion

        public override object VisitRawAssembly([NotNull] CMinusParser.RawAssemblyContext context) {
            this.writer.WriteRaw(context.ASSEMBLY().GetText());
            return null;
        }

        public override object VisitVariableDeclaration_Variable([NotNull] CMinusParser.VariableDeclaration_VariableContext context) {
            this.symbolTable.AddVariable(context.ID().GetText(), 1);
            return null;
        }

        public override object VisitVariableDeclaration_Array([NotNull] CMinusParser.VariableDeclaration_ArrayContext context) {
            string arrayName = context.ID().GetText();
            int arraySize = int.Parse(context.NUM().GetText());

            this.symbolTable.AddVariable(arrayName, arraySize + 1);

            int arrayPosition = this.symbolTable.GetVariableIndex(arrayName);

            this.writer.WriteVariableAddress($"{arrayName}[0]", arrayPosition + 1);

            this.writer.WriteVariableAddress(arrayName, arrayPosition);
            this.writer.WriteContextRegisterRead();
            this.writer.WriteBinaryArithmeticExpression("+");

            this.writer.WriteMemoryWrite();

            return null;
        }

        // TODO
        public override object VisitTypeSpecifier([NotNull] CMinusParser.TypeSpecifierContext context) {
            return base.VisitTypeSpecifier(context);
        }

        public override object VisitParameter_Array([NotNull] CMinusParser.Parameter_ArrayContext context) {
            string parameterName = context.ID().GetText();
            this.symbolTable.AddVariable(parameterName, 1);

            int parameterIndex = this.symbolTable.GetVariableIndex(parameterName);
            this.writer.WriteContextRegisterRead();
            this.writer.WriteBinaryArithmeticExpression("+");
            this.writer.WriteMemoryWrite();

            return null;
        }

        public override object VisitParameter_Variable([NotNull] CMinusParser.Parameter_VariableContext context) {
            string parameterName = context.ID().GetText();
            this.symbolTable.AddVariable(parameterName, 1);

            int parameterIndex = this.symbolTable.GetVariableIndex(parameterName);
            this.writer.WriteVariableAddress(parameterName, parameterIndex);

            this.writer.WriteContextRegisterRead();
            this.writer.WriteBinaryArithmeticExpression("+");
            this.writer.WriteMemoryWrite();

            return null;
        }

        public override object VisitParameterList_ManyParameters([NotNull] CMinusParser.ParameterList_ManyParametersContext context) {
            this.Visit(context.parameter());
            this.Visit(context.parameterList());
            return null;
        }

        public override object VisitFunctionDeclaration([NotNull] CMinusParser.FunctionDeclarationContext context) {

            string functionLabel = this.labelGenerator.GenerateFunctionLabel(context.ID().GetText());
            this.writer.WriteLabel(functionLabel);

            this.EnterContext();
            this.Visit(context.parameters());

            this.Visit(context.compoundStatement());
            this.ExitContext();

            return null;
        }

        public void EnterContext() {
            this.writer.WriteContextRegisterRead();
            this.writer.WriteImmediate(this.symbolTable.contextStack.Peek().size);
            this.writer.WriteBinaryArithmeticExpression("+");
            this.writer.WriteContextRegisterWrite();

            this.symbolTable.EnterContext();
        }

        public void ExitContext() {
            if (this.symbolTable.contextStack.Count > 1) {
                this.symbolTable.ExitContext();

                this.writer.WriteContextRegisterRead();
                this.writer.WriteImmediate(this.symbolTable.contextStack.Peek().size);
                this.writer.WriteBinaryArithmeticExpression("-");
                this.writer.WriteContextRegisterWrite();
            }
        }

        public void ExitAllContexts() {
            while (this.symbolTable.contextStack.Count > 1) {
                this.ExitContext();
            }
        }

        public override object VisitCompoundStatement([NotNull] CMinusParser.CompoundStatementContext context) {
            this.EnterContext();
            base.VisitCompoundStatement(context);
            this.ExitContext();

            return null;
        }

        public override object VisitAccessVariable([NotNull] CMinusParser.AccessVariableContext context) {
            this.Visit(context.variable());
            this.writer.WriteMemoryAccess();

            return null;
        }

        public override object VisitAssignmentVariable([NotNull] CMinusParser.AssignmentVariableContext context) {
            this.Visit(context.variable());
            this.writer.WriteMemoryWrite();

            return null;
        }

        public override object VisitExpressionStatement([NotNull] CMinusParser.ExpressionStatementContext context) {

            if (context.assignmentVariable() != null) {
                this.Visit(context.logicalOrExpression());
                this.Visit(context.assignmentVariable());
            }

            return null;
        }

        public override object VisitSelectionStatement([NotNull] CMinusParser.SelectionStatementContext context) {

            string endLabel = this.labelGenerator.GenerateIfLabel();

            this.Visit(context.logicalOrExpression());

            if (context.elseStatement != null) {
                string elseLabel = this.labelGenerator.GenerateIfLabel();

                this.writer.WriteConditionalJump(elseLabel);
                this.Visit(context.ifStatement);
                this.writer.WriteUnconditionalJump(endLabel);

                this.writer.WriteLabel(elseLabel);
                this.Visit(context.elseStatement);
            }
            else {
                this.writer.WriteConditionalJump(endLabel);
                this.Visit(context.ifStatement);
            }

            this.writer.WriteLabel(endLabel);

            this.labelGenerator.IncrementIfCount();

            return null;
        }

        public override object VisitIterationStatement([NotNull] CMinusParser.IterationStatementContext context) {

            string expressionLabel = this.labelGenerator.GenerateWhileConditionLabel();
            string loopLabel = this.labelGenerator.GenerateWhileLabel();

            this.writer.WriteUnconditionalJump(expressionLabel);

            this.writer.WriteLabel(loopLabel);
            this.Visit(context.statement());

            this.writer.WriteLabel(expressionLabel);
            this.Visit(context.logicalOrExpression());

            this.writer.WriteConditionalJump(loopLabel);

            this.labelGenerator.IncrementWhileCount();

            return null;
        }

        public override object VisitReturnStatement([NotNull] CMinusParser.ReturnStatementContext context) {

            if (context.logicalOrExpression() != null)
                this.Visit(context.logicalOrExpression());

            this.ExitAllContexts();

            this.writer.WriteFunctionExit();

            return null;
        }

        public override object VisitVariable_Pointer([NotNull] CMinusParser.Variable_PointerContext context) {
            this.Visit(context.variable());
            this.writer.WriteMemoryAccess();

            return null;
        }

        public override object VisitVariable_ArrayAccess([NotNull] CMinusParser.Variable_ArrayAccessContext context) {
            this.Visit(context.variable());
            this.writer.WriteMemoryAccess();
            this.Visit(context.logicalOrExpression());
            this.writer.WriteBinaryArithmeticExpression("+");

            return null;
        }

        public override object VisitVariable_ID([NotNull] CMinusParser.Variable_IDContext context) {

            string variableName = context.ID().GetText();
            int variableIndex = this.symbolTable.GetVariableIndex(variableName);

            this.writer.WriteVariableAddress(variableName, variableIndex);
            this.writer.WriteContextRegisterRead();
            this.writer.WriteBinaryArithmeticExpression("+");

            return null;
        }

        public override object VisitUnaryExpression([NotNull] CMinusParser.UnaryExpressionContext context) {

            string unaryOperator = context.children[0].GetText();

            switch (unaryOperator) {
                case "-":
                case "~":
                case "!":
                case "&": {
                    this.Visit(context.factor());
                    this.writer.WriteUnaryArithmeticExpression(unaryOperator);
                    break;
                }
            }

            return null;
        }

        public override object VisitFactor([NotNull] CMinusParser.FactorContext context) {
            if (context.NUM() != null) {
                this.writer.WriteImmediate(context.NUM().GetText());
            }
            else {
                base.VisitFactor(context);
            }

            return null;
        }

        public override object VisitLogicalOrExpression_NoOr([NotNull] CMinusParser.LogicalOrExpression_NoOrContext context) {
            this.inExpression++;
            base.VisitLogicalOrExpression_NoOr(context);
            this.inExpression--;

            return null;
        }

        // TODO
        public override object VisitLogicalOrExpression_Or([NotNull] CMinusParser.LogicalOrExpression_OrContext context) {
            this.inExpression++;
            base.VisitLogicalOrExpression_Or(context);
            this.inExpression--;

            return null;
        }

        // TODO
        public override object VisitLogicalAndExpression_And([NotNull] CMinusParser.LogicalAndExpression_AndContext context) {
            return base.VisitLogicalAndExpression_And(context);
        }

        public override object VisitBitwiseExpression_Bitwise([NotNull] CMinusParser.BitwiseExpression_BitwiseContext context) {

            this.Visit(context.bitwiseExpression());
            this.Visit(context.comparisonExpressionEquals());

            this.writer.WriteBinaryArithmeticExpression(context.children[1].GetText());

            return null;
        }

        public override object VisitComparisonExpressionEquals_Equals([NotNull] CMinusParser.ComparisonExpressionEquals_EqualsContext context) {

            this.Visit(context.comparisonExpression());
            this.Visit(context.comparisonExpressionEquals());

            this.writer.WriteBinaryArithmeticExpression(context.children[1].GetText());

            return null;
        }

        public override object VisitComparisonExpression_Comparison([NotNull] CMinusParser.ComparisonExpression_ComparisonContext context) {

            this.Visit(context.comparisonExpression());
            this.Visit(context.shiftExpression());

            this.writer.WriteBinaryArithmeticExpression(context.children[1].GetText());

            return null;
        }

        public override object VisitShiftExpression_Shift([NotNull] CMinusParser.ShiftExpression_ShiftContext context) {

            this.Visit(context.shiftExpression());
            this.Visit(context.sumExpression());

            this.writer.WriteBinaryArithmeticExpression(context.children[1].GetText());

            return null;
        }

        public override object VisitSumExpression_Sum([NotNull] CMinusParser.SumExpression_SumContext context) {

            this.Visit(context.sumExpression());
            this.Visit(context.multiplyExpression());

            this.writer.WriteBinaryArithmeticExpression(context.children[1].GetText());

            return null;
        }

        public override object VisitMultiplyExpression_Multiplication([NotNull] CMinusParser.MultiplyExpression_MultiplicationContext context) {

            this.Visit(context.multiplyExpression());
            this.Visit(context.factor());

            this.writer.WriteBinaryArithmeticExpression(context.children[1].GetText());

            return null;
        }

        // TODO - DROP if out of expression and function called returns int
        public override object VisitFunctionCall([NotNull] CMinusParser.FunctionCallContext context) {

            if (context.argumentList() != null)
                this.Visit(context.argumentList());

            string functionName = context.ID().GetText();
            string functionLabel = this.labelGenerator.GenerateFunctionLabel(functionName);

            this.writer.WriteFunctionCall(functionLabel);

            return null;
        }

        private void ThrowCompilerException(string exception) {
            throw new CompilerException(exception);
        }

        public class CompilerException : InvalidOperationException {
            public CompilerException() : base() { }
            public CompilerException(string exception) : base(exception) { }
            public CompilerException(string exception, Exception innerException) : base(exception, innerException) { }
        }

        public class MiniSymbolTable {

            public struct SymbolEntry {
                public string SymbolName;
                public int SymbolIndex;
                public int SymbolSize;
            }

            public class Context {
                public Dictionary<string, SymbolEntry> symbols;
                public int size;
            }

            public readonly Stack<Context> contextStack = new Stack<Context>();
            private int totalSize = 0;

            public void EnterContext() {

                if (this.contextStack.Count > 0) {
                    this.totalSize += this.contextStack.Peek().size;
                }

                Context emptyContext = new Context {
                    size = 0,
                    symbols = new Dictionary<string, SymbolEntry>()
                };

                this.contextStack.Push(emptyContext);
            }

            public void AddVariable(string variableName, int size) {
                SymbolEntry symbol;
                symbol.SymbolName = variableName;
                symbol.SymbolIndex = this.contextStack.Peek().size;
                symbol.SymbolSize = size;

                this.contextStack.Peek().size += size;

                this.contextStack.Peek().symbols.Add(variableName, symbol);
            }

            public int GetVariableIndex(string variableName) {

                int totalSizeDiff = 0;
                bool skippedCurrentContext = false;

                foreach (Context ctx in this.contextStack) {
                    if (skippedCurrentContext) totalSizeDiff += ctx.size;

                    if (ctx.symbols.ContainsKey(variableName)) {
                        if (totalSizeDiff > 0) {
                            return ctx.symbols[variableName].SymbolIndex - totalSizeDiff;
                        }
                        else {
                            return ctx.symbols[variableName].SymbolIndex;
                        }
                    }

                    skippedCurrentContext = true;
                }

                return -1;
            }

            public void ExitContext() {

                this.contextStack.Pop();

                if (this.contextStack.Count > 0) {
                    this.totalSize -= this.contextStack.Peek().size;
                }

            }

        }

    }
}
