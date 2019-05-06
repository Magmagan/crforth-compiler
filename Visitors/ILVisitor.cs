using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CrimsonForthCompiler.Grammar;

namespace CrimsonForthCompiler.Visitors {

    class ILVisitor : CMinusBaseVisitor<object> {

        private readonly SymbolTable symbolTable;
        int visitCount = 0;

        private readonly ILWriter writer = new ILWriter();

        public ILVisitor (SymbolTable symbolTable) {
            this.symbolTable = symbolTable;
        }

        public override object VisitArgumentList([NotNull] CMinusParser.ArgumentListContext context) {
            return base.VisitArgumentList(context);
        }

        public override object VisitIterationStatement([NotNull] CMinusParser.IterationStatementContext context) {



            return null;
        }

        public override object VisitFactor([NotNull] CMinusParser.FactorContext context) {
            if (this.visitCount > 0)
                Console.WriteLine(context.GetText());
            return base.VisitFactor(context);
        }

        #region Arithmetic Expressions

        public override object VisitLogicalOrExpression_Or([NotNull] CMinusParser.LogicalOrExpression_OrContext context) {

            this.visitCount++;

            this.Visit(context.logicalOrExpression());
            this.Visit(context.logicalAndExpression());

            this.visitCount--;

            Console.WriteLine(context.children[1].GetText() + "\n");

            return null;
        }

        public override object VisitLogicalAndExpression_And([NotNull] CMinusParser.LogicalAndExpression_AndContext context) {

            this.visitCount++;

            this.Visit(context.logicalAndExpression());
            this.Visit(context.bitwiseExpression());

            this.visitCount--;

            Console.WriteLine(context.children[1].GetText() + "\n");

            return null;
        }

        public override object VisitBitwiseExpression_Bitwise([NotNull] CMinusParser.BitwiseExpression_BitwiseContext context) {

            this.visitCount++;

            this.Visit(context.bitwiseExpression());
            this.Visit(context.comparisonExpressionEquals());

            this.visitCount--;

            Console.WriteLine(context.children[1].GetText() + "\n");

            return null;
        }

        public override object VisitComparisonExpressionEquals_Equals([NotNull] CMinusParser.ComparisonExpressionEquals_EqualsContext context) {

            this.visitCount++;

            this.Visit(context.comparisonExpressionEquals());
            this.Visit(context.comparisonExpression());

            this.visitCount--;

            Console.WriteLine(context.children[1].GetText() + "\n");

            return null;
        }

        public override object VisitComparisonExpression_Comparison([NotNull] CMinusParser.ComparisonExpression_ComparisonContext context) {

            this.visitCount++;

            this.Visit(context.comparisonExpression());
            this.Visit(context.shiftExpression());

            this.visitCount--;

            Console.WriteLine(context.children[1].GetText() + "\n");

            return null;
        }

        public override object VisitShiftExpression_Shift([NotNull] CMinusParser.ShiftExpression_ShiftContext context) {

            this.visitCount++;

            this.Visit(context.shiftExpression());
            this.Visit(context.sumExpression());

            this.visitCount--;

            Console.WriteLine(context.children[1].GetText() + "\n");

            return null;
        }

        public override object VisitSumExpression_Sum([NotNull] CMinusParser.SumExpression_SumContext context) {

            this.visitCount++;

            this.Visit(context.sumExpression());
            this.Visit(context.multiplyExpression());

            this.visitCount--;

            Console.WriteLine(context.children[1].GetText() + "\n");

            return null;
        }

        public override object VisitMultiplyExpression_Multiplication([NotNull] CMinusParser.MultiplyExpression_MultiplicationContext context) {

            this.visitCount++;

            this.Visit(context.multiplyExpression());
            this.Visit(context.factor());

            this.visitCount--;

            Console.WriteLine(context.children[1].GetText() + "\n");

            return null;
        }

        #endregion

        public override object VisitExpressionStatement([NotNull] CMinusParser.ExpressionStatementContext context) {

            Console.WriteLine("# " + context.GetText());
            return base.VisitExpressionStatement(context);
        }

    }

}
