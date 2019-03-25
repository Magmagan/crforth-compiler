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

        static int Main(string[] args) {

            Console.Write("File path: ");
            string path = Console.ReadLine().Trim('\"');

            string input = File.ReadAllText(path);
            AntlrInputStream inputStream = new AntlrInputStream(new StringReader(input));
            CMinusLexer lexer = new CMinusLexer(inputStream);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            CMinusParser parser = new CMinusParser(tokens);

            SyntaxErrorListener syntaxErrorListener = new SyntaxErrorListener();
            parser.AddErrorListener(syntaxErrorListener);

            CMinusParser.CompileUnitContext tree = parser.compileUnit();

            if (lexer.errors > 0) 
                Console.WriteLine("Lexical analysis failure.");

            if (syntaxErrorListener.errors > 0)
                Console.WriteLine("Syntax analysis failure.");

            if (lexer.errors > 0 || syntaxErrorListener.errors > 0) {
                Console.ReadKey();
                return -1;
            }

            GlobalAnalysisVisitor globalVisitor = new GlobalAnalysisVisitor();
            globalVisitor.Visit(tree);

            if (globalVisitor.errors > 0) {
                Console.Error.WriteLine("Semantic analysis failure.");
                Console.ReadKey();
                return -1;
            }

            Console.Write(input);
            Console.ReadKey();

            return 0;
        }
    }

}
