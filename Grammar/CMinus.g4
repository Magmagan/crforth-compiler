grammar CMinus;

/*
 * Parser Rules
 */

// ILVisitor OK
// CRVisitor OK
program
    : declarationList
    ;

// ILVisitor
// CRVisitor
declarationList
    : declaration
    | declarationList declaration
    ;

// ILVisitor
// CRVisitor
declaration //OK
    : variableDeclaration
    | functionDeclaration
    | structDeclaration
    ;

// ILVisitor
// CRVisitor
structDeclaration //OK
    : 'struct' ID '{' structDeclarationList '}'
    ;

// ILVisitor
// CRVisitor
structDeclarationList //OK
    : structVariableDeclaration #structDeclarationList_OneDeclaration
    | structDeclarationList structVariableDeclaration #structDeclarationList_ManyDeclarations
    ;

// ILVisitor
// CRVisitor
structVariableDeclaration //OK
    : typeSpecifier ID ';' #structVariableDeclaration_Variable
    | typeSpecifier ID '[' NUM ']' ';' #structVariableDeclaration_Array
    ;

// ILVisitor
// CRVisitor
variableDeclaration //OK
    : typeSpecifier ID ';' #variableDeclaration_Variable
    | typeSpecifier ID '[' NUM ']' ';' #variableDeclaration_Array
    ;

// ILVisitor
// CRVisitor
typeSpecifier
    : 'int'
    | 'void'
    | 'struct' ID
    | typeSpecifier pointer
    ;

// ILVisitor
// CRVisitor
pointer
    : '*'
    | pointer '*'
    ;

// ILVisitor
// CRVisitor
functionDeclaration //OK
    : typeSpecifier ID '(' parameters ')' compoundStatement
    ;

// ILVisitor
// CRVisitor
parameters //OK
    : parameterList #parameters_WithParameterList
    | 'void' #parameters_Void
    ;

// ILVisitor
// CRVisitor
parameterList //OK
    : parameter #parameterList_OneParameter
    | parameterList ',' parameter #parameterList_ManyParameters
    ;

// ILVisitor
// CRVisitor
parameter //OK
    : typeSpecifier ID #parameter_Variable
    | typeSpecifier ID '[' ']' #parameter_Array
    ;

// ILVisitor
// CRVisitor
compoundStatement //OK
    : '{' statementList? '}'
    ;

// ILVisitor
// CRVisitor
statementList //OK
    : statement
    | statementList statement
    ;

// ILVisitor
// CRVisitor
statement
    : expressionStatement
    | compoundStatement
    | selectionStatement
    | iterationStatement
    | returnStatement
    | variableDeclaration
    | functionCall
    ;

// ILVisitor
// CRVisitor
expressionStatement //OK
    : variable '=' logicalOrExpression ';'
    | ';'
    ;

// ILVisitor
// CRVisitor
selectionStatement //OK
    : 'if' '(' logicalOrExpression ')' ifStatement=statement
    | 'if' '(' logicalOrExpression ')' ifStatement=statement ('else' elseStatement=statement)
    ;

// ILVisitor
// CRVisitor
iterationStatement //OK
    : 'while' '(' logicalOrExpression ')' statement
    ;

// ILVisitor
// CRVisitor
returnStatement //OK
    : 'return' logicalOrExpression? ';'
    ;

// ILVisitor
// CRVisitor
variable
    : '*' variable #variable_Pointer
    | variable '.' variable #variable_StructAccess
    | variable '[' logicalOrExpression ']' #variable_ArrayAccess
    | ID #variable_ID //OK
    ;

// ILVisitor
// CRVisitor
unaryExpression
    : '&' factor
    | '-' factor
    | '~' factor
    | '!' factor
    ;
// ILVisitor
// CRVisitor

factor
    : '(' logicalOrExpression ')'
    | unaryExpression
    | variable
    | functionCall
    | NUM
    ;

// ILVisitor
// CRVisitor
logicalOrExpression
    : logicalOrExpression '||' logicalAndExpression #logicalOrExpression_Or
    | logicalAndExpression #logicalOrExpression_NoOr
    ;

// ILVisitor
// CRVisitor
logicalAndExpression
    : logicalAndExpression '&&' bitwiseExpression #logicalAndExpression_And
    | bitwiseExpression #logicalAndExpression_NoAnd
    ;

// ILVisitor
// CRVisitor
bitwiseExpression
    : bitwiseExpression ('&'|'^'|'|') comparisonExpressionEquals #bitwiseExpression_Bitwise
    | comparisonExpressionEquals #bitwiseExpression_NoBitwise
    ;

// ILVisitor
// CRVisitor
comparisonExpressionEquals
    : comparisonExpressionEquals ('=='|'!=') comparisonExpression #comparisonExpressionEquals_Equals
    | comparisonExpression #comparisonExpressionEquals_NoEquals
    ;

// ILVisitor
// CRVisitor
comparisonExpression
    : comparisonExpression ('<='|'<'|'>'|'>=') shiftExpression #comparisonExpression_Comparison
    | shiftExpression #comparisonExpression_NoComparison
    ;

// ILVisitor
// CRVisitor
shiftExpression
    : shiftExpression ('>>'|'<<') sumExpression #shiftExpression_Shift
    | sumExpression #shiftExpression_NoShift
    ;

// ILVisitor
// CRVisitor
sumExpression
    : sumExpression ('+'|'-') multiplyExpression #sumExpression_Sum
    | multiplyExpression #sumExpression_NoSum
    ;

// ILVisitor
// CRVisitor
multiplyExpression
    : multiplyExpression ('*'|'/'|'%') factor #multiplyExpression_Multiplication
    | factor #multiplyExpression_NoMultiplication
    ;

// ILVisitor
// CRVisitor
functionCall //OK
    : ID '(' argumentList? ')'
    ;

// ILVisitor
// CRVisitor
argumentList
    : logicalOrExpression
    | argumentList ',' logicalOrExpression
    ;

// ILVisitor
// CRVisitor
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