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
using CrimsonForthCompiler.Visitors;

namespace CrimsonForthCompiler {

    class Program {

        static int Main(string[] args) {

            ILWriter writer = new ILWriter();
            writer.Test();

            Console.ReadKey();
            return 0;

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
                Console.Error.WriteLine("Global semantic analysis failure.");
                Console.ReadKey();
                return -1;
            }

            InternalAnalysisVisitor internalVisitor = new InternalAnalysisVisitor(globalVisitor.symbolTable);
            internalVisitor.Visit(tree);

            if (internalVisitor.errors> 0) {
                Console.Error.WriteLine("Internal semantic analysis failure.");
                Console.ReadKey();
                return -1;
            }

            Console.WriteLine("\n----------\n");

            ILVisitor ilVisitor = new ILVisitor(globalVisitor.symbolTable);
            ilVisitor.Visit(tree);

            Console.WriteLine("\n----------\n");

            Console.ReadKey();

            return 0;
        }
    }

}
