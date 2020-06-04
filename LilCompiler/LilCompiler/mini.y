
// Uwaga: W wywołaniu generatora gppg należy użyć opcji /gplex

%namespace GardensPoint

%output=Parser.cs

%union
{
public string      val;
public SyntaxNode  node;
}

%token Program If Else While Read Write Semicolon Assign Eof Or And BitOr BitAnd Equality NotEquality Greater
%token GreaterOrE Less LessOrE Plus Minus Multiplies Divides Not BitNot OpenPar ClosePar OpenCurl CloseCurl
%token IntDecl DoubleDecl BoolDecl Error Return
%token <val> True False Ident IntNum DoubleNum StringVal

%type <node> decllist decl prog stmnt exp bit unar term

%%

start     : Program OpenCurl decllist Eof
          ;

decllist  : decl decllist 
          | prog 
          ;

decl      : BoolDecl Ident Semicolon
               { Compiler.code.Add(new DeclIdentNode($2, CType.Bool)); }
          | IntDecl Ident Semicolon
               { Compiler.code.Add(new DeclIdentNode($2, CType.Int)); }
          | DoubleDecl Ident Semicolon
               { Compiler.code.Add(new DeclIdentNode($2, CType.Double)); }
          ;

prog      : stmnt prog
          | CloseCurl { YYACCEPT; }
          ;

stmnt     : exp Semicolon
               {
               Compiler.code.Add($1);
               Compiler.code.Add(new SemicolonNode());
               }
          | Return Semicolon
               { Compiler.code.Add(new ReturnNode()); }
          | Write StringVal Semicolon
               { Compiler.code.Add(new WriteStrNode($2)); }
          | Write exp Semicolon
               { Compiler.code.Add(new WriteNode($2)); }
          | Read Ident Semicolon
               { Compiler.code.Add(new ReadNode($2)); }
          ;

exp       : bit
               { $$ = $1; }
          | Ident Assign exp
               { $$ = new AssignNode($1, $3); }
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

