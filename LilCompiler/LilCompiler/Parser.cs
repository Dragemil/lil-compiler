// This code was generated by the Gardens Point Parser Generator
// Copyright (c) Wayne Kelly, John Gough, QUT 2005-2014
// (see accompanying GPPGcopyright.rtf)

// GPPG version 1.5.2
// Machine:  DESKTOP-BGFGHK4
// DateTime: 04.06.2020 22:02:23
// UserName: drage
// Input file <mini.y - 04.06.2020 22:02:04>

// options: lines gplex

using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Text;
using QUT.Gppg;

namespace GardensPoint
{
public enum Tokens {error=2,EOF=3,Program=4,If=5,Else=6,
    While=7,Read=8,Write=9,Semicolon=10,Assign=11,Eof=12,
    Or=13,And=14,BitOr=15,BitAnd=16,Equality=17,NotEquality=18,
    Greater=19,GreaterOrE=20,Less=21,LessOrE=22,Plus=23,Minus=24,
    Multiplies=25,Divides=26,Not=27,BitNot=28,OpenPar=29,ClosePar=30,
    OpenCurl=31,CloseCurl=32,IntDecl=33,DoubleDecl=34,BoolDecl=35,Error=36,
    Return=37,True=38,False=39,Ident=40,IntNum=41,DoubleNum=42,
    StringVal=43};

public struct ValueType
#line 9 "mini.y"
{
public string      val;
public SyntaxNode  node;
}
#line default
// Abstract base class for GPLEX scanners
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
public abstract class ScanBase : AbstractScanner<ValueType,LexLocation> {
  private LexLocation __yylloc = new LexLocation();
  public override LexLocation yylloc { get { return __yylloc; } set { __yylloc = value; } }
  protected virtual bool yywrap() { return true; }
}

// Utility class for encapsulating token information
[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
public class ScanObj {
  public int token;
  public ValueType yylval;
  public LexLocation yylloc;
  public ScanObj( int t, ValueType val, LexLocation loc ) {
    this.token = t; this.yylval = val; this.yylloc = loc;
  }
}

[GeneratedCodeAttribute( "Gardens Point Parser Generator", "1.5.2")]
public class Parser: ShiftReduceParser<ValueType, LexLocation>
{
#pragma warning disable 649
  private static Dictionary<int, string> aliases;
#pragma warning restore 649
  private static Rule[] rules = new Rule[32];
  private static State[] states = new State[64];
  private static string[] nonTerms = new string[] {
      "decllist", "decl", "prog", "stmnt", "exp", "bit", "unar", "term", "start", 
      "$accept", };

  static Parser() {
    states[0] = new State(new int[]{4,3},new int[]{-9,1});
    states[1] = new State(new int[]{3,2});
    states[2] = new State(-1);
    states[3] = new State(new int[]{31,4});
    states[4] = new State(new int[]{35,9,33,12,34,15,29,27,38,33,39,34,41,35,42,36,40,48,24,38,27,40,28,42,37,53,9,55,8,60,32,63},new int[]{-1,5,-2,7,-3,18,-4,19,-5,21,-6,23,-7,47,-8,26});
    states[5] = new State(new int[]{12,6});
    states[6] = new State(-2);
    states[7] = new State(new int[]{35,9,33,12,34,15,29,27,38,33,39,34,41,35,42,36,40,48,24,38,27,40,28,42,37,53,9,55,8,60,32,63},new int[]{-1,8,-2,7,-3,18,-4,19,-5,21,-6,23,-7,47,-8,26});
    states[8] = new State(-3);
    states[9] = new State(new int[]{40,10});
    states[10] = new State(new int[]{10,11});
    states[11] = new State(-5);
    states[12] = new State(new int[]{40,13});
    states[13] = new State(new int[]{10,14});
    states[14] = new State(-6);
    states[15] = new State(new int[]{40,16});
    states[16] = new State(new int[]{10,17});
    states[17] = new State(-7);
    states[18] = new State(-4);
    states[19] = new State(new int[]{29,27,38,33,39,34,41,35,42,36,40,48,24,38,27,40,28,42,37,53,9,55,8,60,32,63},new int[]{-3,20,-4,19,-5,21,-6,23,-7,47,-8,26});
    states[20] = new State(-8);
    states[21] = new State(new int[]{10,22});
    states[22] = new State(-10);
    states[23] = new State(new int[]{15,24,16,51,10,-15,30,-15});
    states[24] = new State(new int[]{29,27,38,33,39,34,41,35,42,36,40,37,24,38,27,40,28,42},new int[]{-7,25,-8,26});
    states[25] = new State(-18);
    states[26] = new State(-20);
    states[27] = new State(new int[]{33,30,34,44,29,27,38,33,39,34,41,35,42,36,40,48,24,38,27,40,28,42},new int[]{-5,28,-6,23,-7,47,-8,26});
    states[28] = new State(new int[]{30,29});
    states[29] = new State(-26);
    states[30] = new State(new int[]{30,31});
    states[31] = new State(new int[]{29,27,38,33,39,34,41,35,42,36,40,37,24,38,27,40,28,42},new int[]{-7,32,-8,26});
    states[32] = new State(-21);
    states[33] = new State(-27);
    states[34] = new State(-28);
    states[35] = new State(-29);
    states[36] = new State(-30);
    states[37] = new State(-31);
    states[38] = new State(new int[]{29,27,38,33,39,34,41,35,42,36,40,37,24,38,27,40,28,42},new int[]{-7,39,-8,26});
    states[39] = new State(-23);
    states[40] = new State(new int[]{29,27,38,33,39,34,41,35,42,36,40,37,24,38,27,40,28,42},new int[]{-7,41,-8,26});
    states[41] = new State(-24);
    states[42] = new State(new int[]{29,27,38,33,39,34,41,35,42,36,40,37,24,38,27,40,28,42},new int[]{-7,43,-8,26});
    states[43] = new State(-25);
    states[44] = new State(new int[]{30,45});
    states[45] = new State(new int[]{29,27,38,33,39,34,41,35,42,36,40,37,24,38,27,40,28,42},new int[]{-7,46,-8,26});
    states[46] = new State(-22);
    states[47] = new State(-17);
    states[48] = new State(new int[]{11,49,15,-31,16,-31,10,-31,30,-31});
    states[49] = new State(new int[]{29,27,38,33,39,34,41,35,42,36,40,48,24,38,27,40,28,42},new int[]{-5,50,-6,23,-7,47,-8,26});
    states[50] = new State(-16);
    states[51] = new State(new int[]{29,27,38,33,39,34,41,35,42,36,40,37,24,38,27,40,28,42},new int[]{-7,52,-8,26});
    states[52] = new State(-19);
    states[53] = new State(new int[]{10,54});
    states[54] = new State(-11);
    states[55] = new State(new int[]{43,56,29,27,38,33,39,34,41,35,42,36,40,48,24,38,27,40,28,42},new int[]{-5,58,-6,23,-7,47,-8,26});
    states[56] = new State(new int[]{10,57});
    states[57] = new State(-12);
    states[58] = new State(new int[]{10,59});
    states[59] = new State(-13);
    states[60] = new State(new int[]{40,61});
    states[61] = new State(new int[]{10,62});
    states[62] = new State(-14);
    states[63] = new State(-9);

    for (int sNo = 0; sNo < states.Length; sNo++) states[sNo].number = sNo;

    rules[1] = new Rule(-10, new int[]{-9,3});
    rules[2] = new Rule(-9, new int[]{4,31,-1,12});
    rules[3] = new Rule(-1, new int[]{-2,-1});
    rules[4] = new Rule(-1, new int[]{-3});
    rules[5] = new Rule(-2, new int[]{35,40,10});
    rules[6] = new Rule(-2, new int[]{33,40,10});
    rules[7] = new Rule(-2, new int[]{34,40,10});
    rules[8] = new Rule(-3, new int[]{-4,-3});
    rules[9] = new Rule(-3, new int[]{32});
    rules[10] = new Rule(-4, new int[]{-5,10});
    rules[11] = new Rule(-4, new int[]{37,10});
    rules[12] = new Rule(-4, new int[]{9,43,10});
    rules[13] = new Rule(-4, new int[]{9,-5,10});
    rules[14] = new Rule(-4, new int[]{8,40,10});
    rules[15] = new Rule(-5, new int[]{-6});
    rules[16] = new Rule(-5, new int[]{40,11,-5});
    rules[17] = new Rule(-6, new int[]{-7});
    rules[18] = new Rule(-6, new int[]{-6,15,-7});
    rules[19] = new Rule(-6, new int[]{-6,16,-7});
    rules[20] = new Rule(-7, new int[]{-8});
    rules[21] = new Rule(-7, new int[]{29,33,30,-7});
    rules[22] = new Rule(-7, new int[]{29,34,30,-7});
    rules[23] = new Rule(-7, new int[]{24,-7});
    rules[24] = new Rule(-7, new int[]{27,-7});
    rules[25] = new Rule(-7, new int[]{28,-7});
    rules[26] = new Rule(-8, new int[]{29,-5,30});
    rules[27] = new Rule(-8, new int[]{38});
    rules[28] = new Rule(-8, new int[]{39});
    rules[29] = new Rule(-8, new int[]{41});
    rules[30] = new Rule(-8, new int[]{42});
    rules[31] = new Rule(-8, new int[]{40});
  }

  protected override void Initialize() {
    this.InitSpecialTokens((int)Tokens.error, (int)Tokens.EOF);
    this.InitStates(states);
    this.InitRules(rules);
    this.InitNonTerminals(nonTerms);
  }

  protected override void DoAction(int action)
  {
#pragma warning disable 162, 1522
    switch (action)
    {
      case 5: // decl -> BoolDecl, Ident, Semicolon
#line 31 "mini.y"
               { Compiler.code.Add(new DeclIdentNode(ValueStack[ValueStack.Depth-2].val, CType.Bool)); }
#line default
        break;
      case 6: // decl -> IntDecl, Ident, Semicolon
#line 33 "mini.y"
               { Compiler.code.Add(new DeclIdentNode(ValueStack[ValueStack.Depth-2].val, CType.Int)); }
#line default
        break;
      case 7: // decl -> DoubleDecl, Ident, Semicolon
#line 35 "mini.y"
               { Compiler.code.Add(new DeclIdentNode(ValueStack[ValueStack.Depth-2].val, CType.Double)); }
#line default
        break;
      case 9: // prog -> CloseCurl
#line 39 "mini.y"
                      { YYAccept(); }
#line default
        break;
      case 10: // stmnt -> exp, Semicolon
#line 43 "mini.y"
               {
               Compiler.code.Add(ValueStack[ValueStack.Depth-2].node);
               Compiler.code.Add(new SemicolonNode());
               }
#line default
        break;
      case 11: // stmnt -> Return, Semicolon
#line 48 "mini.y"
               { Compiler.code.Add(new ReturnNode()); }
#line default
        break;
      case 12: // stmnt -> Write, StringVal, Semicolon
#line 50 "mini.y"
               { Compiler.code.Add(new WriteStrNode(ValueStack[ValueStack.Depth-2].val)); }
#line default
        break;
      case 13: // stmnt -> Write, exp, Semicolon
#line 52 "mini.y"
               { Compiler.code.Add(new WriteNode(ValueStack[ValueStack.Depth-2].node)); }
#line default
        break;
      case 14: // stmnt -> Read, Ident, Semicolon
#line 54 "mini.y"
               { Compiler.code.Add(new ReadNode(ValueStack[ValueStack.Depth-2].val)); }
#line default
        break;
      case 15: // exp -> bit
#line 58 "mini.y"
               { CurrentSemanticValue.node = ValueStack[ValueStack.Depth-1].node; }
#line default
        break;
      case 16: // exp -> Ident, Assign, exp
#line 60 "mini.y"
               { CurrentSemanticValue.node = new AssignNode(ValueStack[ValueStack.Depth-3].val, ValueStack[ValueStack.Depth-1].node); }
#line default
        break;
      case 18: // bit -> bit, BitOr, unar
#line 65 "mini.y"
               { CurrentSemanticValue.node = new BitOrNode(ValueStack[ValueStack.Depth-3].node, ValueStack[ValueStack.Depth-1].node); }
#line default
        break;
      case 19: // bit -> bit, BitAnd, unar
#line 67 "mini.y"
               { CurrentSemanticValue.node = new BitAndNode(ValueStack[ValueStack.Depth-3].node, ValueStack[ValueStack.Depth-1].node); }
#line default
        break;
      case 21: // unar -> OpenPar, IntDecl, ClosePar, unar
#line 72 "mini.y"
               { CurrentSemanticValue.node = new IntConversionNode(ValueStack[ValueStack.Depth-1].node); }
#line default
        break;
      case 22: // unar -> OpenPar, DoubleDecl, ClosePar, unar
#line 74 "mini.y"
               { CurrentSemanticValue.node = new DoubleConversionNode(ValueStack[ValueStack.Depth-1].node); }
#line default
        break;
      case 23: // unar -> Minus, unar
#line 76 "mini.y"
               { CurrentSemanticValue.node = new NegNode(ValueStack[ValueStack.Depth-1].node); }
#line default
        break;
      case 24: // unar -> Not, unar
#line 78 "mini.y"
               { CurrentSemanticValue.node = new BoolNegNode(ValueStack[ValueStack.Depth-1].node); }
#line default
        break;
      case 25: // unar -> BitNot, unar
#line 80 "mini.y"
               { CurrentSemanticValue.node = new BitNegNode(ValueStack[ValueStack.Depth-1].node); }
#line default
        break;
      case 26: // term -> OpenPar, exp, ClosePar
#line 84 "mini.y"
               { CurrentSemanticValue.node = ValueStack[ValueStack.Depth-2].node; }
#line default
        break;
      case 27: // term -> True
#line 86 "mini.y"
               { CurrentSemanticValue.node = new ConstBoolLeaf(true); }
#line default
        break;
      case 28: // term -> False
#line 88 "mini.y"
               { CurrentSemanticValue.node = new ConstBoolLeaf(false); }
#line default
        break;
      case 29: // term -> IntNum
#line 90 "mini.y"
               { CurrentSemanticValue.node = new ConstIntLeaf(int.Parse(ValueStack[ValueStack.Depth-1].val)); }
#line default
        break;
      case 30: // term -> DoubleNum
#line 92 "mini.y"
               { CurrentSemanticValue.node = new ConstDoubleLeaf(double.Parse(ValueStack[ValueStack.Depth-1].val,System.Globalization.CultureInfo.InvariantCulture)); }
#line default
        break;
      case 31: // term -> Ident
#line 94 "mini.y"
               { CurrentSemanticValue.node = new IdentLeaf(ValueStack[ValueStack.Depth-1].val); }
#line default
        break;
    }
#pragma warning restore 162, 1522
  }

  protected override string TerminalToString(int terminal)
  {
    if (aliases != null && aliases.ContainsKey(terminal))
        return aliases[terminal];
    else if (((Tokens)terminal).ToString() != terminal.ToString(CultureInfo.InvariantCulture))
        return ((Tokens)terminal).ToString();
    else
        return CharToString((char)terminal);
  }

#line 98 "mini.y"

public Parser(Scanner scanner) : base(scanner) { }

#line default
}
}
