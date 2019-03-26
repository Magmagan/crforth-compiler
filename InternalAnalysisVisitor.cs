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
        private bool inAssignment = false;
        private string assignmentType = "";
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

        #endregion

        #region Variable Declaration & Use

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

        public override object VisitVariable_Pointer([NotNull] CMinusParser.Variable_PointerContext context) {
            return base.VisitVariable_Pointer(context);
        }

        public override object VisitVariable_StructAccess([NotNull] CMinusParser.Variable_StructAccessContext context) {
            return base.VisitVariable_StructAccess(context);
        }

        public override object VisitVariable_ArrayAccess([NotNull] CMinusParser.Variable_ArrayAccessContext context) {

            string arrayType = (string) this.Visit(context.variable());

            string arrayName = SymbolTable.Symbol.RemoveExtras(context.variable().GetText());

            if (!this.symbolTable.HasSymbol(arrayName)) {
                this.EmitSemanticErrorMessage($"Variable {arrayName} called but not declared", context);
                return SymbolTable.Symbol.Construct.ERROR;
            }

            SymbolTable.Symbol foundSymbol = this.symbolTable.GetSymbol(arrayName);

            if (foundSymbol.construct != SymbolTable.Symbol.Construct.ARRAY) {
                this.EmitSemanticErrorMessage($"{arrayName} used as an array but declared as a {foundSymbol.construct}", context);
                return SymbolTable.Symbol.Construct.ERROR;
            }

            return foundSymbol.type;
        }

        public override object VisitVariable_ID([NotNull] CMinusParser.Variable_IDContext context) {

            string variableName = context.ID().GetText();

            if (!this.symbolTable.HasSymbol(variableName)) {
                this.EmitSemanticErrorMessage($"Variable {variableName} called but not declared", context);
                return "error";
            }

            SymbolTable.Symbol foundSymbol = this.symbolTable.GetSymbol(variableName);

            if (foundSymbol.construct == SymbolTable.Symbol.Construct.FUNCTION || foundSymbol.construct == SymbolTable.Symbol.Construct.ERROR) {
                this.EmitSemanticErrorMessage($"{variableName} used as a variable but declared as a {foundSymbol.construct}", context);
                return "error";
            }

            return foundSymbol.type;
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

        #region Expressions

        public override object VisitExpressionStatement([NotNull] CMinusParser.ExpressionStatementContext context) {
            /*
            string leftHandType = (string) this.Visit(context.variable());

            this.inAssignment = true;
            this.assignmentType = leftHandType;

            string rightHandType = (string) this.Visit(context.logicalOrExpression());

            this.inAssignment = false;
            this.assignmentType = "";

            if (leftHandType != rightHandType) {
                this.EmitSemanticErrorMessage($"Assignment of {leftHandType} expected, but found {rightHandType}", context);
            }

            */
            return null;
            
        }

        #endregion

        private void EmitSemanticErrorMessage(string message, ParserRuleContext context) {
            Console.Error.WriteLine($"Sem | Line {context.Start.Line}:{context.Start.Column} - {message}");
            this.errors++;
        }

    }

}
