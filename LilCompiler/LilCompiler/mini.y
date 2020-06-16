
// Uwaga: W wywołaniu generatora gppg należy użyć opcji /gplex

%namespace GardensPoint

%output=Parser.cs

%union
{
public string      val;
public SyntaxNode  node;
}

%token Program If Else While Read Write Semicolon Assign Or And BitOr BitAnd Equality NotEquality Greater
%token GreaterOrE Less LessOrE Plus Minus Multiplies Divides Not BitNot OpenPar ClosePar OpenCurl CloseCurl
%token IntDecl DoubleDecl BoolDecl Error Return
%token <val> True False Ident IntNum DoubleNum StringVal

%type <node> decllist decl prog stmnt blckstmnt exp log rel comp factor bit unar term

%%

start     : Program OpenCurl decllist
          ;

decllist  : decl decllist 
               { Compiler.AddStatement($1); }
          | prog 
          ;

decl      : BoolDecl Ident Semicolon
               { $$ = new DeclIdentNode($2, CType.Bool); }
          | IntDecl Ident Semicolon
               { $$ = new DeclIdentNode($2, CType.Int); }
          | DoubleDecl Ident Semicolon
               { $$ = new DeclIdentNode($2, CType.Double); }
          ;

prog      : stmnt prog
               { Compiler.AddStatement($1); }
          | CloseCurl
          ;

stmnt     : exp Semicolon
               { $$ = new SemicolonNode($1); }
          | OpenCurl blckstmnt
               { Compiler.scopes.Pop(); $$ = $2; }
          | If OpenPar exp ClosePar stmnt
               { $$ = new IfNode($3, $5); }
          | If OpenPar exp ClosePar stmnt Else stmnt
               { $$ = new IfElseNode($3, $5, $7); }
          | While OpenPar exp ClosePar stmnt
               { $$ = new WhileNode($3, $5); }
          | Return Semicolon
               { $$ = new ReturnNode(); }
          | Write StringVal Semicolon
               { $$ = new WriteStrNode($2); }
          | Write exp Semicolon
               { $$ = new WriteNode($2); }
          | Read Ident Semicolon
               { $$ = new ReadNode($2); }
          ;

blckstmnt : stmnt blckstmnt
               { Compiler.AddStatement($1); $$ = $2; }
          | CloseCurl
               { var scope = new ScopeNode(); Compiler.scopes.Push(scope); $$ = scope; }
          ;

exp       : log
          | Ident Assign exp
               { $$ = new AssignNode($1, $3); }
          ;

log       : rel
          | log Or rel
               { $$ = new OrNode($1, $3); }
          | log And rel
               { $$ = new AndNode($1, $3); }
          ;

rel       : comp
          | rel Equality comp
               { $$ = new EqualityNode($1, $3); }
          | rel NotEquality comp
               { $$ = new NotEqualityNode($1, $3); }
          | rel Greater comp
               { $$ = new GreaterNode($1, $3); }
          | rel GreaterOrE comp
               { $$ = new GreaterOrEqualNode($1, $3); }
          | rel Less comp
               { $$ = new LessNode($1, $3); }
          | rel LessOrE comp
               { $$ = new LessOrEqualNode($1, $3); }
          ;

comp      : factor
          | comp Plus factor
               { $$ = new AdditionNode($1, $3); }
          | comp Minus factor
               { $$ = new SubtractionNode($1, $3); }
          ;

factor    : bit
          | factor Multiplies bit
               { $$ = new MultiplicationNode($1, $3); }
          | factor Divides bit
               { $$ = new DivisionNode($1, $3); }
          ;

bit       : unar
          | bit BitOr unar
               { $$ = new BitOrNode($1, $3); }
          | bit BitAnd unar
               { $$ = new BitAndNode($1, $3); }
          ;

unar      : term
          | OpenPar IntDecl ClosePar unar
               { $$ = new IntConversionNode($4); }
          | OpenPar DoubleDecl ClosePar unar
               { $$ = new DoubleConversionNode($4); }
          | Minus unar
               { $$ = new NegNode($2); }
          | Not unar
               { $$ = new BoolNegNode($2); }
          | BitNot unar
               { $$ = new BitNegNode($2); }
          ;

term      : OpenPar exp ClosePar
               { $$ = $2; }
          | True
               { $$ = new ConstBoolLeaf(true); }
          | False
               { $$ = new ConstBoolLeaf(false); }
          | IntNum
               { $$ = new ConstIntLeaf(int.Parse($1)); }
          | DoubleNum
               { $$ = new ConstDoubleLeaf(double.Parse($1,System.Globalization.CultureInfo.InvariantCulture)); }
          | Ident
               { $$ = new IdentLeaf($1); }
          ;

%%

public Parser(Scanner scanner) : base(scanner) { }

