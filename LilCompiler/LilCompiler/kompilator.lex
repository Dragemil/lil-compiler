
%using QUT.Gppg;
%namespace GardensPoint

IntNum      ([1-9][0-9]*|0)
DoubleNum   ([1-9][0-9]*|0)\.[0-9]+
StringVal   \"([^"\\\r\n]*(\\.[^"\\\r\n]*)*)\"
Ident       [a-zA-Z][a-zA-Z0-9]*
NewLine     (\r\n?|\n)
Comment     \/\/[^\r\n]*

%%

"program"     { return (int)Tokens.Program; }
"if"          { return (int)Tokens.If; }
"else"        { return (int)Tokens.Else; }
"while"       { return (int)Tokens.While; }
"read"        { return (int)Tokens.Read; }
"write"       { return (int)Tokens.Write; }
"return"      { return (int)Tokens.Return; }
"int"         { return (int)Tokens.IntDecl; }
"double"      { return (int)Tokens.DoubleDecl; }
"bool"        { return (int)Tokens.BoolDecl; }
"true"        { yylval.val=yytext; return (int)Tokens.True; }
"false"       { yylval.val=yytext; return (int)Tokens.False; }
{IntNum}      { yylval.val=yytext; return (int)Tokens.IntNum; }
{DoubleNum}   { yylval.val=yytext; return (int)Tokens.DoubleNum; }
{StringVal}   { yylval.val=@yytext; return (int)Tokens.StringVal; }
{Ident}       { yylval.val=yytext; return (int)Tokens.Ident; }
"="           { return (int)Tokens.Assign; }
"||"		  { return (int)Tokens.Or; }
"&&"		  { return (int)Tokens.And; }
"|"		      { return (int)Tokens.BitOr; }
"&"		      { return (int)Tokens.BitAnd; }
"=="		  { return (int)Tokens.Equality; }
"!="		  { return (int)Tokens.NotEquality; }
">"		      { return (int)Tokens.Greater; }
">="		  { return (int)Tokens.GreaterOrE; }
"<"		      { return (int)Tokens.Less; }
"<="		  { return (int)Tokens.LessOrE; }
"+"           { return (int)Tokens.Plus; }
"-"           { return (int)Tokens.Minus; }
"*"           { return (int)Tokens.Multiplies; }
"/"           { return (int)Tokens.Divides; }
"!"           { return (int)Tokens.Not; }
"~"           { return (int)Tokens.BitNot; }
"("           { return (int)Tokens.OpenPar; }
")"           { return (int)Tokens.ClosePar; }
"{"           { return (int)Tokens.OpenCurl; }
"}"           { return (int)Tokens.CloseCurl; }
";"           { return (int)Tokens.Semicolon; }
{NewLine}     { lineno++; }
" "           { }
"\t"          { }
{Comment}     { }

%%

public static int lineno = 1;
