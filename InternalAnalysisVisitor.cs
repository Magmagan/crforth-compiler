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

    class InternalAnalysisVisitor : CMinusBaseVisitor<object> {

        public uint errors;
        public uint internalScope = 0;
        private readonly SymbolTable symbolTable;

        public InternalAnalysisVisitor (SymbolTable symbolTable) {
            this.symbolTable = symbolTable;
        }

        #region Structs

        #endregion

        #region Functions

        public override object VisitFunctionDeclaration([NotNull] CMinusParser.FunctionDeclarationContext context) {

            string functionName = context.ID().GetText();

            SymbolTable.Symbol functionSymbol = this.symbolTable.GetSymbol(functionName);

            this.internalScope++;
            this.symbolTable.EnterScope();

            foreach(SymbolTable.Symbol member in functionSymbol.submembers) {
                bool success = this.symbolTable.AddSymbol(member);
                if (!success) {
                    this.EmitSemanticErrorMessage($"Symbol {member.id} already in symbol table as a {this.symbolTable.GetSymbol(member.id).construct}", context);
                }
            }

            this.Visit(context.compoundStatement());

            this.symbolTable.ExitScope();
            this.internalScope--;

            return true;
        }

        #endregion

        #region Variable Declaration

        public override object VisitVariableDeclaration_Variable([NotNull] CMinusParser.VariableDeclaration_VariableContext context) {

            if (this.internalScope == 0)
                return null;

            string variableName = context.ID().GetText();
            string variableType = SymbolTable.Symbol.RemoveExtras(context.typeSpecifier().GetText());

            if (variableType == "void") {
                this.EmitSemanticErrorMessage("Variable declared as void type", context);
            }

            SymbolTable.Symbol symbol = new SymbolTable.Symbol(
                id: variableName,
                type: variableType,
                construct: SymbolTable.Symbol.Construct.VARIABLE,
                scope: this.internalScope,
                size: 1,
                pointerCount: SymbolTable.Symbol.CountStringAsterisks(context.typeSpecifier().GetText())
            );

            bool success = this.symbolTable.AddSymbol(symbol);

            if (!success) {
                this.EmitSemanticErrorMessage($"Symbol {variableName} already in symbol table as a {this.symbolTable.GetSymbol(variableName).construct}", context);
            }

            return null;
        }

        public override object VisitVariableDeclaration_Array([NotNull] CMinusParser.VariableDeclaration_ArrayContext context) {

            if (this.internalScope == 0)
                return null;

            string variableName = context.ID().GetText();
            string variableType = SymbolTable.Symbol.RemoveExtras(context.typeSpecifier().GetText());

            if (variableType == "void") {
                this.EmitSemanticErrorMessage("Variable declared as void type", context);
            }

            SymbolTable.Symbol symbol = new SymbolTable.Symbol(
                id: variableName,
                type: variableType,
                construct: SymbolTable.Symbol.Construct.ARRAY,
                scope: this.internalScope,
                size: uint.Parse(context.NUM().GetText()),
                pointerCount: SymbolTable.Symbol.CountStringAsterisks(context.typeSpecifier().GetText())
            );

            bool success = this.symbolTable.AddSymbol(symbol);

            if (!success) {
                this.EmitSemanticErrorMessage($"Symbol {variableName} already in symbol table as a {this.symbolTable.GetSymbol(variableName).construct}", context);
            }

            return null;
        }

        #endregion

        #region Control blocks

        public override object VisitSelectionStatement([NotNull] CMinusParser.SelectionStatementContext context) {

            this.Visit(context.logicalOrExpression());

            this.internalScope++;
            this.symbolTable.EnterScope();

            this.Visit(context.ifStatement);

            this.symbolTable.ExitScope();
            this.internalScope--;

            if (context.elseStatement != null) {
                this.internalScope++;
                this.symbolTable.EnterScope();

                this.Visit(context.elseStatement);

                this.symbolTable.ExitScope();
                this.internalScope--;
            }

            return null;
        }

        public override object VisitIterationStatement([NotNull] CMinusParser.IterationStatementContext context) {

            this.Visit(context.logicalOrExpression());

            this.internalScope++;
            this.symbolTable.EnterScope();

            this.Visit(context.statement());

            this.symbolTable.ExitScope();
            this.internalScope--;

            return null;
        }

        #endregion

        public override object VisitFunctionCall([NotNull] CMinusParser.FunctionCallContext context) {

            string functionName = context.ID().GetText();

            if (!this.symbolTable.HasSymbol(functionName)) {
                this.EmitSemanticErrorMessage($"Function {functionName} called but not declared", context);
            }
            else if (this.symbolTable.GetSymbol(functionName).construct != SymbolTable.Symbol.Construct.FUNCTION) {
                this.EmitSemanticErrorMessage($"{functionName} called as function but declared as a {this.symbolTable.GetSymbol(functionName).construct}", context);
            }

            return null;
        }

        private void EmitSemanticErrorMessage(string message, ParserRuleContext context) {
            Console.Error.WriteLine($"Sem | Line {context.Start.Line}:{context.Start.Column} - {message}");
            this.errors++;
        }

    }

}
