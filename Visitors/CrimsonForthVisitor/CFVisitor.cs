using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CrimsonForthCompiler.Grammar;

namespace CrimsonForthCompiler.Visitors.CrimsonForthVisitor {
    class CrimsonForthVisitor : CMinusBaseVisitor<object> {

        public override object VisitProgram([NotNull] CMinusParser.ProgramContext context) {
            this.Visit(context.declarationList());
            return null;
        }

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
        public override object VisitTypeSpecifier([NotNull] CMinusParser.TypeSpecifierContext context) {
            return base.VisitTypeSpecifier(context);
        }

        // TODO
        public override object VisitPointer([NotNull] CMinusParser.PointerContext context) {
            return base.VisitPointer(context);
        }

        // TODO
        public override object VisitFunctionDeclaration([NotNull] CMinusParser.FunctionDeclarationContext context) {
            return base.VisitFunctionDeclaration(context);
        }

        // TODO
        public override object VisitExpressionStatement([NotNull] CMinusParser.ExpressionStatementContext context) {
            return base.VisitExpressionStatement(context);
        }

        // TODO
        public override object VisitSelectionStatement([NotNull] CMinusParser.SelectionStatementContext context) {
            return base.VisitSelectionStatement(context);
        }

        // TODO
        public override object VisitIterationStatement([NotNull] CMinusParser.IterationStatementContext context) {
            return base.VisitIterationStatement(context);
        }

        // TODO
        public override object VisitReturnStatement([NotNull] CMinusParser.ReturnStatementContext context) {
            return base.VisitReturnStatement(context);
        }

        // TODO
        public override object VisitVariable_Pointer([NotNull] CMinusParser.Variable_PointerContext context) {
            return base.VisitVariable_Pointer(context);
        }

        // TODO
        public override object VisitVariable_StructAccess([NotNull] CMinusParser.Variable_StructAccessContext context) {
            return base.VisitVariable_StructAccess(context);
        }

        // TODO
        public override object VisitVariable_ArrayAccess([NotNull] CMinusParser.Variable_ArrayAccessContext context) {
            return base.VisitVariable_ArrayAccess(context);
        }

        // TODO
        public override object VisitVariable_ID([NotNull] CMinusParser.Variable_IDContext context) {
            return base.VisitVariable_ID(context);
        }

        // TODO
        public override object VisitUnaryExpression([NotNull] CMinusParser.UnaryExpressionContext context) {
            return base.VisitUnaryExpression(context);
        }

        // TODO
        public override object VisitFactor([NotNull] CMinusParser.FactorContext context) {
            return base.VisitFactor(context);
        }

        // TODO
        public override object VisitLogicalOrExpression_Or([NotNull] CMinusParser.LogicalOrExpression_OrContext context) {
            return base.VisitLogicalOrExpression_Or(context);
        }

        // TODO
        public override object VisitLogicalAndExpression_And([NotNull] CMinusParser.LogicalAndExpression_AndContext context) {
            return base.VisitLogicalAndExpression_And(context);
        }

        // TODO
        public override object VisitBitwiseExpression_Bitwise([NotNull] CMinusParser.BitwiseExpression_BitwiseContext context) {
            return base.VisitBitwiseExpression_Bitwise(context);
        }

        // TODO
        public override object VisitComparisonExpressionEquals_Equals([NotNull] CMinusParser.ComparisonExpressionEquals_EqualsContext context) {
            return base.VisitComparisonExpressionEquals_Equals(context);
        }

        // TODO
        public override object VisitComparisonExpression_Comparison([NotNull] CMinusParser.ComparisonExpression_ComparisonContext context) {
            return base.VisitComparisonExpression_Comparison(context);
        }

        // TODO
        public override object VisitShiftExpression_Shift([NotNull] CMinusParser.ShiftExpression_ShiftContext context) {
            return base.VisitShiftExpression_Shift(context);
        }

        // TODO
        public override object VisitSumExpression_Sum([NotNull] CMinusParser.SumExpression_SumContext context) {
            return base.VisitSumExpression_Sum(context);
        }

        // TODO
        public override object VisitMultiplyExpression_Multiplication([NotNull] CMinusParser.MultiplyExpression_MultiplicationContext context) {
            return base.VisitMultiplyExpression_Multiplication(context);
        }

        // TODO
        public override object VisitFunctionCall([NotNull] CMinusParser.FunctionCallContext context) {
            return base.VisitFunctionCall(context);
        }

        // TODO
        public override object VisitArgumentList([NotNull] CMinusParser.ArgumentListContext context) {
            return base.VisitArgumentList(context);
        }

    }
}
