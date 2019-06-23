grammar CMinus;

/*
 * Parser Rules
 */

// ILVisitor OK
program
    : declarationList
    ;

// ILVisitor OK
declarationList
    : declaration
    | declarationList declaration
    ;

// ILVisitor OK
declaration
    : variableDeclaration
    | functionDeclaration
    | structDeclaration
    | rawAssembly
    ;

// ILVisitor
// CRVisitor
structDeclaration
    : 'struct' ID '{' structDeclarationList '}'
    ;

// ILVisitor
// CRVisitor
structDeclarationList
    : structVariableDeclaration #structDeclarationList_OneDeclaration
    | structDeclarationList structVariableDeclaration #structDeclarationList_ManyDeclarations
    ;

// ILVisitor
// CRVisitor
structVariableDeclaration
    : typeSpecifier ID ';' #structVariableDeclaration_Variable
    | typeSpecifier ID '[' NUM ']' ';' #structVariableDeclaration_Array
    ;

// ILVisitor
variableDeclaration
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
functionDeclaration
    : typeSpecifier ID '(' parameters ')' compoundStatement
    ;

// ILVisitor
parameters
    : parameterList #parameters_WithParameterList
    | 'void' #parameters_Void
    ;

// ILVisitor
parameterList
    : parameter #parameterList_OneParameter
    | parameterList ',' parameter #parameterList_ManyParameters
    ;

// ILVisitor
parameter
    : typeSpecifier ID #parameter_Variable
    | typeSpecifier ID '[' ']' #parameter_Array
    ;

// ILVisitor
compoundStatement
    : '{' statementList? '}'
    ;

// ILVisitor
statementList
    : statement
    | statementList statement
    ;

// ILVisitor
// CRVisitor
statement
    : compoundStatement
    | expressionStatement
    | selectionStatement
    | iterationStatement
    | returnStatement
    | variableDeclaration
    | rawAssembly
    | functionCall ';'
    ;

// ILVisitor
expressionStatement
    : assignmentVariable '=' logicalOrExpression ';'
    | ';'
    ;

// ILVisitor
selectionStatement
    : 'if' '(' logicalOrExpression ')' ifStatement=statement
    | 'if' '(' logicalOrExpression ')' ifStatement=statement ('else' elseStatement=statement)
    ;

// ILVisitor
iterationStatement
    : 'while' '(' logicalOrExpression ')' statement
    ;

// ILVisitor
returnStatement
    : 'return' logicalOrExpression? ';'
    ;

// ILVisitor
assignmentVariable
    : variable
    ;

// ILVisitor
accessVariable
    : variable
    ;

// ILVisitor
variable
    : '*' variable #variable_Pointer
    | variable '.' variable #variable_StructAccess
    | variable '[' logicalOrExpression ']' #variable_ArrayAccess
    | ID #variable_ID
    ;

// ILVisitor
unaryExpression
    : ('&'|'-'|'~'|'!') factor
    ;

// ILVisitor
factor
    : '(' logicalOrExpression ')'
    | unaryExpression
    | accessVariable
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
bitwiseExpression
    : bitwiseExpression ('&'|'^'|'|') comparisonExpressionEquals #bitwiseExpression_Bitwise
    | comparisonExpressionEquals #bitwiseExpression_NoBitwise
    ;

// ILVisitor
comparisonExpressionEquals
    : comparisonExpressionEquals ('=='|'!=') comparisonExpression #comparisonExpressionEquals_Equals
    | comparisonExpression #comparisonExpressionEquals_NoEquals
    ;

// ILVisitor
comparisonExpression
    : comparisonExpression ('<='|'<'|'>'|'>=') shiftExpression #comparisonExpression_Comparison
    | shiftExpression #comparisonExpression_NoComparison
    ;

// ILVisitor
shiftExpression
    : shiftExpression ('>>'|'<<') sumExpression #shiftExpression_Shift
    | sumExpression #shiftExpression_NoShift
    ;

// ILVisitor
sumExpression
    : sumExpression ('+'|'-') multiplyExpression #sumExpression_Sum
    | multiplyExpression #sumExpression_NoSum
    ;

// ILVisitor
multiplyExpression
    : multiplyExpression ('*'|'/'|'%') factor #multiplyExpression_Multiplication
    | factor #multiplyExpression_NoMultiplication
    ;

// ILVisitor
functionCall
    : ID '(' argumentList? ')'
    ;

// ILVisitor
argumentList
    : logicalOrExpression
    | argumentList ',' logicalOrExpression
    ;

// ILVisitor
rawAssembly
    : ASSEMBLY
    ;

// ILVisitor
compileUnit
	: program EOF
	;


/*
 * Lexer Rules
 */

ASSEMBLY
    : '$' .*? '$'
    ;

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