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

        #region Structs + members

        public override object VisitStructDeclaration([NotNull] CMinusParser.StructDeclarationContext context) {

            string structType = context.ID().GetText();
            this.symbolTable.AddSymbolType(structType);

            List<SymbolTable.Symbol> members = (List<SymbolTable.Symbol>) this.Visit(context.structDeclarationList());

            SymbolTable.Symbol structSymbol = new SymbolTable.Symbol(
                id: structType,
                type: structType,
                construct: SymbolTable.Symbol.Construct.STRUCT,
                scope: 0,
                size: 0, // Visit members
                pointerCount: 0
            );

            return base.VisitStructDeclaration(context);
        }

        public override object VisitStructDeclarationList_OneDeclaration([NotNull] CMinusParser.StructDeclarationList_OneDeclarationContext context) {
            List<SymbolTable.Symbol> members = new List<SymbolTable.Symbol> {
                (SymbolTable.Symbol) this.Visit(context.variableDeclaration())
            };
            return members;
        }

        public override object VisitStructDeclarationList_ManyDeclarations([NotNull] CMinusParser.StructDeclarationList_ManyDeclarationsContext context) {
            List<SymbolTable.Symbol> members = (List<SymbolTable.Symbol>) this.Visit(context.structDeclarationList());
            SymbolTable.Symbol member = (SymbolTable.Symbol) this.Visit(context.variableDeclaration());
            members.Add(member);
            return members;
        }

        #endregion

        #region Variable declaration

        public override object VisitVariableDeclaration_Variable([NotNull] CMinusParser.VariableDeclaration_VariableContext context) {
            return new SymbolTable.Symbol(
                id: context.ID().GetText(),
                type: SymbolTable.Symbol.RemoveExtras(context.typeSpecifier().GetText()),
                construct: SymbolTable.Symbol.Construct.VARIABLE,
                scope: 1,
                size: 1,
                pointerCount: SymbolTable.Symbol.CountStringAsterisks(context.typeSpecifier().GetText())
            );
        }

        public override object VisitVariableDeclaration_Array([NotNull] CMinusParser.VariableDeclaration_ArrayContext context) {
            return new SymbolTable.Symbol(
                id: context.ID().GetText(),
                type: SymbolTable.Symbol.RemoveExtras(context.typeSpecifier().GetText()),
                construct: SymbolTable.Symbol.Construct.VARIABLE,
                scope: 1,
                size: uint.Parse(context.NUM().GetText()),
                pointerCount: SymbolTable.Symbol.CountStringAsterisks(context.typeSpecifier().GetText())
            );
        }

        #endregion

        #region Functions + parameters

        public override object VisitFunctionDeclaration([NotNull] CMinusParser.FunctionDeclarationContext context) {

            string functionType = SymbolTable.Symbol.RemoveExtras(context.typeSpecifier().GetText());
            string functionName = context.ID().GetText();

            List<SymbolTable.Symbol> parameters = (List<SymbolTable.Symbol>) this.Visit(context.parameters()); // Visit parameters

            SymbolTable.Symbol functionSymbol = new SymbolTable.Symbol(
                id: functionName,
                type: functionType,
                construct: SymbolTable.Symbol.Construct.FUNCTION,
                size: (uint) parameters.Count,
                scope: this.internalScope,
                pointerCount: 0
            );
            
            functionSymbol.AddMembers(parameters);

            this.symbolTable.AddSymbol(functionSymbol);

            Console.WriteLine(functionSymbol.ToString());

            this.internalScope++;
            this.VisitCompoundStatement(context.compoundStatement());
            this.internalScope--;

            return null;
        }

        public override object VisitParameters_Void([NotNull] CMinusParser.Parameters_VoidContext context) {
            Console.WriteLine("wat");
            return new List<SymbolTable.Symbol>();
        }

        public override object VisitParameters_WithParameterList([NotNull] CMinusParser.Parameters_WithParameterListContext context) {
            Console.WriteLine("wat???");
            return this.Visit(context.parameterList());
        }

        public override object VisitParameterList_OneParameter([NotNull] CMinusParser.ParameterList_OneParameterContext context) {
            List<SymbolTable.Symbol> parameters = new List<SymbolTable.Symbol> {
                (SymbolTable.Symbol) this.Visit(context.parameter())
            };
            Console.WriteLine($"Here: {parameters.Count}");
            return parameters;
        }

        public override object VisitParameterList_ManyParameters([NotNull] CMinusParser.ParameterList_ManyParametersContext context) {
            List<SymbolTable.Symbol> parameters = (List<SymbolTable.Symbol>) this.Visit(context.parameterList());
            SymbolTable.Symbol parameter = (SymbolTable.Symbol) this.Visit(context.parameter());
            parameters.Add(parameter);
            return parameters;
        }

        public override object VisitParameter_Variable([NotNull] CMinusParser.Parameter_VariableContext context) {
            return new SymbolTable.Symbol(
                id: context.ID().GetText(),
                type: SymbolTable.Symbol.RemoveExtras(context.typeSpecifier().GetText()),
                construct: SymbolTable.Symbol.Construct.VARIABLE,
                scope: 1,
                size: 1,
                pointerCount: SymbolTable.Symbol.CountStringAsterisks(context.typeSpecifier().GetText())
            );
        }

        public override object VisitParameter_Array([NotNull] CMinusParser.Parameter_ArrayContext context) {
            return new SymbolTable.Symbol(
                id: context.ID().GetText(),
                type: SymbolTable.Symbol.RemoveExtras(context.typeSpecifier().GetText()),
                construct: SymbolTable.Symbol.Construct.ARRAY,
                scope: 1,
                size: 1,
                pointerCount: SymbolTable.Symbol.CountStringAsterisks(context.typeSpecifier().GetText())
            );
        }

        #endregion

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
            this.Visit(context.program());
            if (!(this.symbolTable.HasSymbol("main") && this.symbolTable.GetSymbol("main").construct == SymbolTable.Symbol.Construct.FUNCTION)) {
                Console.WriteLine("BAD");
                Console.Error.WriteLine("No main function!");
            }
            return null;
        }

    }
}
