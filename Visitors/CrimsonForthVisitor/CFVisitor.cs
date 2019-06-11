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



    }
}
