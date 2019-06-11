grammar CMinus;

/*
 * Parser Rules
 */

program //OK
    : declarationList
    ;

declarationList //OK
    : declaration
    | declarationList declaration
    ;

declaration //OK
    : variableDeclaration
    | functionDeclaration
    | structDeclaration
    ;

structDeclaration //OK
    : 'struct' ID '{' structDeclarationList '}'
    ;

structDeclarationList //OK
    : structVariableDeclaration #structDeclarationList_OneDeclaration
    | structDeclarationList structVariableDeclaration #structDeclarationList_ManyDeclarations
    ;

structVariableDeclaration //OK
    : typeSpecifier ID ';' #structVariableDeclaration_Variable
    | typeSpecifier ID '[' NUM ']' ';' #structVariableDeclaration_Array
    ;

variableDeclaration //OK
    : typeSpecifier ID ';' #variableDeclaration_Variable
    | typeSpecifier ID '[' NUM ']' ';' #variableDeclaration_Array
    ;

typeSpecifier
    : 'int'
    | 'void'
    | 'struct' ID
    | typeSpecifier pointer
    ;

pointer
    : '*'
    | pointer '*'
    ;

functionDeclaration //OK
    : typeSpecifier ID '(' parameters ')' compoundStatement
    ;

parameters //OK
    : parameterList #parameters_WithParameterList
    | 'void' #parameters_Void
    ;

parameterList //OK
    : parameter #parameterList_OneParameter
    | parameterList ',' parameter #parameterList_ManyParameters
    ;

parameter //OK
    : typeSpecifier ID #parameter_Variable
    | typeSpecifier ID '[' ']' #parameter_Array
    ;

compoundStatement //OK
    : '{' statementList? '}'
    ;

statementList //OK
    : statement
    | statementList statement
    ;

statement
    : expressionStatement
    | compoundStatement
    | selectionStatement
    | iterationStatement
    | returnStatement
    | variableDeclaration
    | functionCall
    ;

expressionStatement //OK
    : variable '=' logicalOrExpression ';'
    | ';'
    ;

selectionStatement //OK
    : 'if' '(' logicalOrExpression ')' ifStatement=statement
    | 'if' '(' logicalOrExpression ')' ifStatement=statement ('else' elseStatement=statement)
    ;

iterationStatement //OK
    : 'while' '(' logicalOrExpression ')' statement
    ;

returnStatement //OK
    : 'return' logicalOrExpression? ';'
    ;

variable
    : '*' variable #variable_Pointer
    | variable '.' variable #variable_StructAccess
    | variable '[' logicalOrExpression ']' #variable_ArrayAccess
    | ID #variable_ID //OK
    ;

unaryExpression
    : '&' factor
    | '-' factor
    | '~' factor
    | '!' factor
    ;

factor
    : '(' logicalOrExpression ')'
    | unaryExpression
    | variable
    | functionCall
    | NUM
    ;

logicalOrExpression
    : logicalOrExpression '||' logicalAndExpression #logicalOrExpression_Or
    | logicalAndExpression #logicalOrExpression_NoOr
    ;

logicalAndExpression
    : logicalAndExpression '&&' bitwiseExpression #logicalAndExpression_And
    | bitwiseExpression #logicalAndExpression_NoAnd
    ;

bitwiseExpression
    : bitwiseExpression ('&'|'^'|'|') comparisonExpressionEquals #bitwiseExpression_Bitwise
    | comparisonExpressionEquals #bitwiseExpression_NoBitwise
    ;

comparisonExpressionEquals
    : comparisonExpressionEquals ('=='|'!=') comparisonExpression #comparisonExpressionEquals_Equals
    | comparisonExpression #comparisonExpressionEquals_NoEquals
    ;

comparisonExpression
    : comparisonExpression ('<='|'<'|'>'|'>=') shiftExpression #comparisonExpression_Comparison
    | shiftExpression #comparisonExpression_NoComparison
    ;

shiftExpression
    : shiftExpression ('>>'|'<<') sumExpression #shiftExpression_Shift
    | sumExpression #shiftExpression_NoShift
    ;

sumExpression
    : sumExpression ('+'|'-') multiplyExpression #sumExpression_Sum
    | multiplyExpression #sumExpression_NoSum
    ;

multiplyExpression
    : multiplyExpression ('*'|'/'|'%') factor #multiplyExpression_Multiplication
    | factor #multiplyExpression_NoMultiplication
    ;

functionCall //OK
    : ID '(' argumentList? ')'
    ;

argumentList
    : logicalOrExpression
    | argumentList ',' logicalOrExpression
    ;

compileUnit
	: program EOF
	;


/*
 * Lexer Rules
 */

ID
    : [a-zA-Z_][a-zA-Z0-9_]*
    ;

NUM
    : [0-9]+
    ;

WHITESPACE
    : [\n\r\t ] -> skip
    ;

BLOCKCOMMENT
    : '/*' .*? '*/'	-> skip
    ;

LINECOMMENT
    : '//' ~'\n'* -> skip
    ;

WS
    : ' ' -> channel(HIDDEN)
    ;

ErrorChar
    : .
    ;