
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

%type <node> exp term stmnt decl prog decllist

%%

start     : Program OpenCurl decllist
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
          | CloseCurl Eof
          ;

stmnt     : exp Semicolon
               {
               Compiler.code.Add($1);
               Compiler.code.Add(new SemicolonNode());
               }
          | Write exp Semicolon
               { Compiler.code.Add(new WriteNode($2)); }
          ;



exp       : term
               { $$ = $1; }
          | Ident Assign exp
               { $$ = new AssignNode($1, $3); }
          ;

term      : True
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

