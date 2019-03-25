grammar CMinus;

/*
 * Parser Rules
 */

program
    : declarationList
    ;

declarationList
    : declaration
    | declarationList declaration
    ;

declaration
    : variableDeclaration
    | functionDeclaration
    | structDeclaration
    ;

structDeclaration
    : 'struct' ID '{' structDeclarationList '}'
    ;

structDeclarationList
    : structVariableDeclaration #structDeclarationList_OneDeclaration
    | structDeclarationList structVariableDeclaration #structDeclarationList_ManyDeclarations
    ;

structVariableDeclaration
    : typeSpecifier ID ';' #structVariableDeclaration_Variable
    | typeSpecifier ID '[' NUM ']' ';' #structVariableDeclaration_Array
    ;

variableDeclaration
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

functionDeclaration
    : typeSpecifier ID '(' parameters ')' compoundStatement
    ;

parameters
    : parameterList #parameters_WithParameterList
    | 'void' #parameters_Void
    ;

parameterList
    : parameter #parameterList_OneParameter
    | parameterList ',' parameter #parameterList_ManyParameters
    ;

parameter
    : typeSpecifier ID #parameter_Variable
    | typeSpecifier ID '[' ']' #parameter_Array
    ;

compoundStatement
    : '{' internalDeclarations? statementList? '}'
    ;

internalDeclarations
    : variableDeclaration
    | internalDeclarations variableDeclaration
    ;

statementList
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
    ;

expressionStatement
    : expression? ';'
    ;

selectionStatement
    : 'if' '(' expression ')' statement ('else' statement)?
    ;

iterationStatement
    : 'while' '(' expression ')' statement
    ;

returnStatement
    : 'return' expressionStatement
    ;

expression
    : variable '=' expression   #expressaoatribuicao
    | logicalOrExpression       #expressaosimples
    ;

variable
    : '*' variable
    | ID '[' expression ']'
    | ID
    ;

unaryExpression
    : '&' factor
    | '*' factor
    | '-' factor
    | '~' factor
    | '!' factor
    ;

factor
    : '(' expression ')'
    | unaryExpression
    | variable
    | functionCall
    | NUM
    ;

logicalOrExpression
    : logicalOrExpression '||' logicalAndExpression
    | logicalAndExpression
    ;

logicalAndExpression
    : logicalAndExpression '&&' bitwiseExpression
    | bitwiseExpression
    ;

bitwiseExpression
    : bitwiseExpression '&' comparisonExpression
    | bitwiseExpression '^' comparisonExpression
    | bitwiseExpression '|' comparisonExpression
    | comparisonExpression
    ;

comparisonExpression
    : comparisonExpression '<=' shiftExpression
    | comparisonExpression '<' shiftExpression
    | comparisonExpression '>' shiftExpression
    | comparisonExpression '>=' shiftExpression
    | comparisonExpression '==' shiftExpression
    | comparisonExpression '!=' shiftExpression
    | shiftExpression
    ;

shiftExpression
    : shiftExpression '>>' sumExpression
    | shiftExpression '<<' sumExpression
    | sumExpression
    ;

sumExpression
    : sumExpression '+' multiplyExpression
    | sumExpression '-' multiplyExpression
    | multiplyExpression
    ;

multiplyExpression
    : multiplyExpression '*' factor
    | multiplyExpression '/' factor
    | multiplyExpression '%' factor
    | factor
    ;

functionCall
    : ID '(' argumentList ')'
    | ID '(' ')'
    ;

argumentList
    : expression
    | argumentList ',' expression
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