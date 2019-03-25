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

    class GlobalAnalysisVisitor : CMinusBaseVisitor<object> {

        private uint internalScope = 0;
        public uint errors = 0;
        public readonly SymbolTable symbolTable = new SymbolTable();

        #region Structs + members

        public override object VisitStructDeclaration([NotNull] CMinusParser.StructDeclarationContext context) {

            string structType = context.ID().GetText();

            bool typeSuccess = this.symbolTable.AddSymbolType(structType);

            if (!typeSuccess) {
                this.EmitSemanticErrorMessage($"Struct type {structType} already declared", context);
            }

            List<SymbolTable.Symbol> members = (List<SymbolTable.Symbol>) this.Visit(context.structDeclarationList());

            uint membersSize = (uint) members.Sum(member => member.size);

            SymbolTable.Symbol structSymbol = new SymbolTable.Symbol(
                id: structType,
                type: structType,
                construct: SymbolTable.Symbol.Construct.STRUCT,
                scope: 0,
                size: membersSize,
                pointerCount: 0
            );

            bool symbolSuccess = this.symbolTable.AddSymbol(structSymbol);

            if (!symbolSuccess) {
                this.EmitSemanticErrorMessage($"Symbol {structType} already in symbol table as a {this.symbolTable.GetSymbol(structType).construct}", context);
            }

            return null;
        }

        public override object VisitStructDeclarationList_OneDeclaration([NotNull] CMinusParser.StructDeclarationList_OneDeclarationContext context) {
            List<SymbolTable.Symbol> members = new List<SymbolTable.Symbol> {
                (SymbolTable.Symbol) this.Visit(context.structVariableDeclaration())
            };
            return members;
        }

        public override object VisitStructDeclarationList_ManyDeclarations([NotNull] CMinusParser.StructDeclarationList_ManyDeclarationsContext context) {
            List<SymbolTable.Symbol> members = (List<SymbolTable.Symbol>) this.Visit(context.structDeclarationList());
            SymbolTable.Symbol member = (SymbolTable.Symbol) this.Visit(context.structVariableDeclaration());
            members.Add(member);
            return members;
        }

        public override object VisitStructVariableDeclaration_Variable([NotNull] CMinusParser.StructVariableDeclaration_VariableContext context) {

            string symbolType = SymbolTable.Symbol.RemoveExtras(context.typeSpecifier().GetText());

            if (symbolType == "void") {
                this.EmitSemanticErrorMessage("Variable declared as void type", context);
            }

            return new SymbolTable.Symbol(
                id: context.ID().GetText(),
                type: symbolType,
                construct: SymbolTable.Symbol.Construct.VARIABLE,
                scope: 1,
                size: 1,
                pointerCount: SymbolTable.Symbol.CountStringAsterisks(context.typeSpecifier().GetText())
            );
        }

        public override object VisitStructVariableDeclaration_Array([NotNull] CMinusParser.StructVariableDeclaration_ArrayContext context) {

            string symbolType = SymbolTable.Symbol.RemoveExtras(context.typeSpecifier().GetText());

            if (symbolType == "void") {
                this.EmitSemanticErrorMessage("Variable declared as void type", context);
            }

            return new SymbolTable.Symbol(
                id: context.ID().GetText(),
                type: symbolType,
                construct: SymbolTable.Symbol.Construct.ARRAY,
                scope: 1,
                size: uint.Parse(context.NUM().GetText()),
                pointerCount: SymbolTable.Symbol.CountStringAsterisks(context.typeSpecifier().GetText())
            );
        }

        #endregion

        #region Variable declaration

        public override object VisitVariableDeclaration_Variable([NotNull] CMinusParser.VariableDeclaration_VariableContext context) {

            string variableName = context.ID().GetText();
            string variableType = SymbolTable.Symbol.RemoveExtras(context.typeSpecifier().GetText());

            if (variableType == "void") {
                this.EmitSemanticErrorMessage("Variable declared as void type", context);
            }

            SymbolTable.Symbol symbol = new SymbolTable.Symbol(
                id: variableName,
                type: variableType,
                construct: SymbolTable.Symbol.Construct.VARIABLE,
                scope: 0,
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

            string variableName = context.ID().GetText();
            string variableType = SymbolTable.Symbol.RemoveExtras(context.typeSpecifier().GetText());

            if (variableType == "void") {
                this.EmitSemanticErrorMessage("Variable declared as void type", context);
            }

            SymbolTable.Symbol symbol = new SymbolTable.Symbol(
                id: variableName,
                type: variableType,
                construct: SymbolTable.Symbol.Construct.ARRAY,
                scope: 0,
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

            bool success = this.symbolTable.AddSymbol(functionSymbol);

            if (!success) {
                this.EmitSemanticErrorMessage($"Symbol {functionName} already in symbol table as a {this.symbolTable.GetSymbol(functionName).construct}", context);
            }

            this.internalScope++;
            this.VisitCompoundStatement(context.compoundStatement());
            this.internalScope--;

            return null;
        }

        public override object VisitParameters_Void([NotNull] CMinusParser.Parameters_VoidContext context) {
            return new List<SymbolTable.Symbol>();
        }

        public override object VisitParameters_WithParameterList([NotNull] CMinusParser.Parameters_WithParameterListContext context) {
            return this.Visit(context.parameterList());
        }

        public override object VisitParameterList_OneParameter([NotNull] CMinusParser.ParameterList_OneParameterContext context) {
            List<SymbolTable.Symbol> parameters = new List<SymbolTable.Symbol> {
                (SymbolTable.Symbol) this.Visit(context.parameter())
            };
            return parameters;
        }

        public override object VisitParameterList_ManyParameters([NotNull] CMinusParser.ParameterList_ManyParametersContext context) {
            List<SymbolTable.Symbol> parameters = (List<SymbolTable.Symbol>) this.Visit(context.parameterList());
            SymbolTable.Symbol parameter = (SymbolTable.Symbol) this.Visit(context.parameter());
            parameters.Add(parameter);
            return parameters;
        }

        public override object VisitParameter_Variable([NotNull] CMinusParser.Parameter_VariableContext context) {

            string symbolType = SymbolTable.Symbol.RemoveExtras(context.typeSpecifier().GetText());

            if (symbolType == "void") {
                this.EmitSemanticErrorMessage("Variable declared as void type", context);
            }

            return new SymbolTable.Symbol(
                id: context.ID().GetText(),
                type: symbolType,
                construct: SymbolTable.Symbol.Construct.VARIABLE,
                scope: 1,
                size: 1,
                pointerCount: SymbolTable.Symbol.CountStringAsterisks(context.typeSpecifier().GetText())
            );
        }

        public override object VisitParameter_Array([NotNull] CMinusParser.Parameter_ArrayContext context) {

            string symbolType = SymbolTable.Symbol.RemoveExtras(context.typeSpecifier().GetText());

            if (symbolType == "void") {
                this.EmitSemanticErrorMessage("Variable declared as void type", context);
            }

            return new SymbolTable.Symbol(
                id: context.ID().GetText(),
                type: symbolType,
                construct: SymbolTable.Symbol.Construct.ARRAY,
                scope: 1,
                size: 1,
                pointerCount: SymbolTable.Symbol.CountStringAsterisks(context.typeSpecifier().GetText())
            );
        }

        #endregion

        public override object VisitCompileUnit([NotNull] CMinusParser.CompileUnitContext context) {
            this.Visit(context.program());
            if (!(this.symbolTable.HasSymbol("main") && this.symbolTable.GetSymbol("main").construct == SymbolTable.Symbol.Construct.FUNCTION)) {
                this.EmitSemanticErrorMessage("No main function found", context);
            }
            return null;
        }

        private void EmitSemanticErrorMessage(string message, ParserRuleContext context) {
            Console.Error.WriteLine($"Sem | Line {context.Start.Line}:{context.Start.Column} - {message}");
            this.errors++;
        }

    }
}
