using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CrimsonForthCompiler.Grammar;

namespace CrimsonForthCompiler.Visitors.ILVisitor {

    class ILVisitor : CMinusBaseVisitor<object> {

        private readonly SymbolTable symbolTable;
        private readonly LabelGenerator labelGenerator = new LabelGenerator();
        int visitCount = 0;

        private readonly ILWriter writer = new ILWriter();

        public ILVisitor (SymbolTable symbolTable) {
            this.symbolTable = symbolTable;
        }

        public override object VisitProgram([NotNull] CMinusParser.ProgramContext context) {
            this.Visit(context.declarationList());
            this.writer.Dump();
            return null;
        }

        public override object VisitFunctionDeclaration([NotNull] CMinusParser.FunctionDeclarationContext context) {

            this.writer.EnterFunction(context.ID().GetText(), context.typeSpecifier().GetText());
            this.Visit(context.compoundStatement());
            this.writer.ExitFunction();

            return null;
        }

        public override object VisitArgumentList([NotNull] CMinusParser.ArgumentListContext context) {
            return base.VisitArgumentList(context);
        }

        public override object VisitVariableDeclaration_Variable([NotNull] CMinusParser.VariableDeclaration_VariableContext context) {
            this.writer.WriteVariableDeclaration(context.ID().GetText());
            return null;
        }

        public override object VisitReturnStatement([NotNull] CMinusParser.ReturnStatementContext context) {
            
            if (context.logicalOrExpression() != null) {
                this.Visit(context.logicalOrExpression());
            }

            this.writer.WriteFunctionReturn();

            return null;
        }

        public override object VisitFactor([NotNull] CMinusParser.FactorContext context) {

            if (context.variable() != null) {
                this.writer.WriteLoadVariable(context.variable().GetText());
            }
            
            if (context.NUM() != null) {
                this.writer.WritePush(int.Parse(context.NUM().GetText()));
            }

            return base.VisitFactor(context);
        }

        #region Control Blocks

        public override object VisitSelectionStatement([NotNull] CMinusParser.SelectionStatementContext context) {

            int selectionEndLabel = this.labelGenerator.GenerateLabel();
            int elseBodyLabel = this.labelGenerator.GenerateLabel();

            this.Visit(context.logicalOrExpression());

            if (context.elseStatement != null) {
                this.writer.WriteJumpIfFalse(elseBodyLabel);
                this.Visit(context.ifStatement);
                this.writer.WriteUnconditionalJump(selectionEndLabel);

                this.writer.WriteLabel(elseBodyLabel);
                this.Visit(context.elseStatement);
            }
            else {
                this.writer.WriteJumpIfFalse(selectionEndLabel);
                this.Visit(context.ifStatement);
            }

            this.writer.WriteLabel(selectionEndLabel);

            return null;
        }

        public override object VisitIterationStatement([NotNull] CMinusParser.IterationStatementContext context) {

            int logicalExpressionLabel = this.labelGenerator.GenerateLabel();
            int statementBodyLabel = this.labelGenerator.GenerateLabel();
            
            this.writer.WriteUnconditionalJump(logicalExpressionLabel);
            
            this.writer.WriteLabel(statementBodyLabel);
            this.Visit(context.statement());
            
            this.writer.WriteLabel(logicalExpressionLabel);
            this.Visit(context.logicalOrExpression());
            
            this.writer.WriteJumpIfTrue(statementBodyLabel);

            return null;
        }

        #endregion

        #region Arithmetic Expressions

        public override object VisitLogicalOrExpression_Or([NotNull] CMinusParser.LogicalOrExpression_OrContext context) {

            this.visitCount++;

            this.Visit(context.logicalOrExpression());
            this.Visit(context.logicalAndExpression());

            this.visitCount--;

            this.writer.WriteArithmeticOperand(context.children[1].GetText());

            return null;
        }

        public override object VisitLogicalAndExpression_And([NotNull] CMinusParser.LogicalAndExpression_AndContext context) {

            this.visitCount++;

            this.Visit(context.logicalAndExpression());
            this.Visit(context.bitwiseExpression());

            this.visitCount--;

            this.writer.WriteArithmeticOperand(context.children[1].GetText());

            return null;
        }

        public override object VisitBitwiseExpression_Bitwise([NotNull] CMinusParser.BitwiseExpression_BitwiseContext context) {

            this.visitCount++;

            this.Visit(context.bitwiseExpression());
            this.Visit(context.comparisonExpressionEquals());

            this.visitCount--;

            this.writer.WriteArithmeticOperand(context.children[1].GetText());

            return null;
        }

        public override object VisitComparisonExpressionEquals_Equals([NotNull] CMinusParser.ComparisonExpressionEquals_EqualsContext context) {

            this.visitCount++;

            this.Visit(context.comparisonExpressionEquals());
            this.Visit(context.comparisonExpression());

            this.visitCount--;

            this.writer.WriteArithmeticOperand(context.children[1].GetText());

            return null;
        }

        public override object VisitComparisonExpression_Comparison([NotNull] CMinusParser.ComparisonExpression_ComparisonContext context) {

            this.visitCount++;

            this.Visit(context.comparisonExpression());
            this.Visit(context.shiftExpression());

            this.visitCount--;

            this.writer.WriteArithmeticOperand(context.children[1].GetText());

            return null;
        }

        public override object VisitShiftExpression_Shift([NotNull] CMinusParser.ShiftExpression_ShiftContext context) {

            this.visitCount++;

            this.Visit(context.shiftExpression());
            this.Visit(context.sumExpression());

            this.visitCount--;

            this.writer.WriteArithmeticOperand(context.children[1].GetText());

            return null;
        }

        public override object VisitSumExpression_Sum([NotNull] CMinusParser.SumExpression_SumContext context) {

            this.visitCount++;

            this.Visit(context.sumExpression());
            this.Visit(context.multiplyExpression());

            this.visitCount--;

            this.writer.WriteArithmeticOperand(context.children[1].GetText());

            return null;
        }

        public override object VisitMultiplyExpression_Multiplication([NotNull] CMinusParser.MultiplyExpression_MultiplicationContext context) {

            this.visitCount++;

            this.Visit(context.multiplyExpression());
            this.Visit(context.factor());

            this.visitCount--;

            this.writer.WriteArithmeticOperand(context.children[1].GetText());

            return null;
        }

        #endregion

        public override object VisitExpressionStatement([NotNull] CMinusParser.ExpressionStatementContext context) {

            if (context.variable() != null) {
                this.Visit(context.logicalOrExpression());
                this.writer.WriteStoreVariable(context.variable().GetText());
            }

            return null;
        }

    }

}
