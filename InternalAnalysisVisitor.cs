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

            this.symbolTable.ExitScope();
            this.internalScope--;

            return true;
        }

        #endregion

        private void EmitSemanticErrorMessage(string message, ParserRuleContext context) {
            Console.Error.WriteLine($"Sem | Line {context.Start.Line}:{context.Start.Column} - {message}");
            this.errors++;
        }

    }

}
