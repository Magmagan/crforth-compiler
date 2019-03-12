using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CrimsonForthCompiler.Grammar;

namespace CrimsonForthCompiler {

    class AnalysisVisitor : CMinusBaseVisitor<object> {

        private uint internalScope = 0;
        private readonly SymbolTable symbolTable = new SymbolTable();

        public override object VisitFunctionDeclaration([NotNull] CMinusParser.FunctionDeclarationContext context) {

            SymbolTable.Symbol.Type functionType = SymbolTable.Symbol.StringToType(context.typeSpecifier().GetText());
            string functionName = context.ID().GetText();
            SymbolTable parameters = null; // Visit parameters

            SymbolTable.Symbol functionSymbol = new SymbolTable.Symbol(
                id: functionName,
                type: functionType,
                construct: SymbolTable.Symbol.Construct.FUNCTION,
                size: 0, // Visit parameters
                scope: this.internalScope
            );

            this.symbolTable.AddSymbol(functionSymbol);

            Console.WriteLine(functionSymbol.ToString());

            this.internalScope++;
            this.VisitCompoundStatement(context.compoundStatement());
            this.internalScope--;

            return null;
        }

        public override object VisitCompoundStatement([NotNull] CMinusParser.CompoundStatementContext context) {
            Console.WriteLine("Im in!");
            return base.VisitCompoundStatement(context);
        }

        public override object VisitMultiplyExpression([NotNull] CMinusParser.MultiplyExpressionContext context) {

            int Icontext = context.children.Count;
            Console.WriteLine("Do Something: " + Icontext);

            if (Icontext == 3) {
                Console.WriteLine($"Child 1: {context.children[0].GetText()}");
                Console.WriteLine($"Child 2: {context.children[1].GetText()}");
                Console.WriteLine($"Child 3: {context.children[2].GetText()}");
            }

            return base.VisitMultiplyExpression(context);
        }

        public override object VisitCompileUnit([NotNull] CMinusParser.CompileUnitContext context) {
            this.VisitChildren(context);
            if (!(this.symbolTable.HasSymbol("main") && this.symbolTable.GetSymbol("main").construct == SymbolTable.Symbol.Construct.FUNCTION)) {
                Console.WriteLine("BAD");
                Console.Error.WriteLine("No main function!");
            }
            return null;
        }

    }
}
