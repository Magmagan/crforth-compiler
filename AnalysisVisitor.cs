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

    class AnalysisVisitor : CMinusBaseVisitor<String> {

        readonly CMinusParser parser;

        public AnalysisVisitor(CMinusParser parser) {
            this.parser = parser;
        }

        public override String VisitMultiplyExpression([NotNull] CMinusParser.MultiplyExpressionContext context) {

            int Icontext = context.children.Count;
            Console.WriteLine("Do Something: " + Icontext);
            if (Icontext == 3) {
                Console.WriteLine($"Child 1: {context.children[0].GetText()}");
                Console.WriteLine($"Child 2: {context.children[1].GetText()}");
                Console.WriteLine($"Child 3: {context.children[2].GetText()}");
            }
            return base.VisitMultiplyExpression(context);
        }

    }
}
