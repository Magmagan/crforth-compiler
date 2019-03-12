using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using CrimsonForthCompiler.Grammar;

namespace CrimsonForthCompiler {

    class Program {

        static void Main(string[] args) {

            Console.Write("File path: ");
            string path = Console.ReadLine().Trim('\"');

            string input = File.ReadAllText(path);
            AntlrInputStream inputStream = new AntlrInputStream(new StringReader(input));
            CMinusLexer lexer = new CMinusLexer(inputStream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            CMinusParser parser = new CMinusParser(tokens);
            CMinusParser.CompileUnitContext tree = parser.compileUnit();

            AnalysisVisitor visitor = new AnalysisVisitor(parser);
            visitor.Visit(tree);

            Console.Write(input);
            Console.ReadKey();

        }
    }
}
