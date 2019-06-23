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
using CrimsonForthCompiler.Visitors.AnalysisVisitors;
using CrimsonForthCompiler.Visitors.IntermediateLanguageVisitor;
using CrimsonForthCompiler.Visitors.CrimsonForthVisitor;

namespace CrimsonForthCompiler {

    class Program {

        static string PrependCommonFunctions(string code) {

            string inputFunction =
                "int input(void) {\n"
                + "    $IN$\n"
                + "    $GrB>$\n"
                + "}\n"
                + "\n";

            string outputFunction =
                "void output(void) {\n"
                + "    $GrB<$\n"
                + "    $OUT$\n"
                + "}\n"
                + "\n";

            string moduloFunction =
                "int _modulo(int n, int m) {\n"
                + "    while(n >= m) {\n"
                + "        n = n - m;\n"
                + "    }\n"
                + "    return n;\n"
                + "}\n"
                + "\n";

            string divisionFunction =
                "int _divide(int n, int m) {\n"
                + "    int r;\n"
                + "    r = 0;\n"
                + "    while(n >= m) {\n"
                + "        n = n - m;\n"
                + "        r = r + 1;\n"
                + "    }\n"
                + "    return r;\n"
                + "}\n"
                + "\n";

            return inputFunction + outputFunction + divisionFunction + moduloFunction + code;
        }

        static int Main(string[] args) {

            Console.Write("File path: ");
            string path = Console.ReadLine().Trim('\"');
            string input = File.ReadAllText(path);
            string code = PrependCommonFunctions(input);

            Console.WriteLine(code);

            AntlrInputStream inputStream = new AntlrInputStream(new StringReader(code));
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

            Console.WriteLine(globalVisitor.symbolTable.ToString());

            InternalAnalysisVisitor internalVisitor = new InternalAnalysisVisitor(globalVisitor.symbolTable);
            internalVisitor.Visit(tree);

            if (internalVisitor.errors> 0) {
                Console.Error.WriteLine("Internal semantic analysis failure.");
                Console.ReadKey();
                return -1;
            }

            Console.WriteLine("\n----------\n");

            // ILVisitor ilVisitor = new ILVisitor(globalVisitor.symbolTable);
            // ilVisitor.Visit(tree);

            CrimsonForthVisitor CFVisitor = new CrimsonForthVisitor();
            CFVisitor.Visit(tree);

            Console.WriteLine(CFVisitor.writer.Finalize());

            Console.WriteLine("\n----------\n");

            Console.ReadKey();

            return 0;
        }
    }

}
