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
    ;

expressionStatement //OK
    : (variable '=' logicalOrExpression)? ';'
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

functionCall //OK
    : ID '(' argumentList ')'
    | ID '(' ')'
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