
using System;
using System.IO;
using System.Collections.Generic;
using GardensPoint;
using System.Linq;

public class Compiler
{
    public static int errors = 0;

    public static Stack<ScopeNode> scopes = new Stack<ScopeNode>(new[] { new ScopeNode() });

    public static Dictionary<string, CType> variables =
        new Dictionary<string, CType>();

    private static int labelNum = 1;

    // arg[0] określa plik źródłowy
    // pozostałe argumenty są ignorowane
    public static int Main(string[] args)
    {
        string file;
        FileStream source;
        Console.WriteLine("\nDouble-Pass CIL Code Generator for Mini Language - Gardens Point");
        if (args.Length >= 1)
            file = args[0];
        else
        {
            Console.Write("\nsource file:  ");
            file = Console.ReadLine();
        }
        try
        {
            var sr = new StreamReader(file);
            string str = sr.ReadToEnd();
            sr.Close();
            source = new FileStream(file, FileMode.Open);
        }
        catch (Exception e)
        {
            Console.WriteLine("\n" + e.Message);
            return 1;
        }

        Scanner scanner = new Scanner(source);
        Parser parser = new Parser(scanner);
        try
        {
            Console.WriteLine();
            if (!parser.Parse())
            {
                throw new ErrorException("Invalid syntax");
            }
        }
        catch (ErrorException) { }

        if (errors == 0)
        {
            sw = new StreamWriter(file + ".il");
            GenCode();
            sw.Close();
            source.Close();
            Console.WriteLine("  compilation successful\n");
        }
        else
        {
            Console.WriteLine($"\n  {errors} errors detected\n");
            File.Delete(file + ".il");
        }
        return errors == 0 ? 0 : 2;
    }

    public static SyntaxNode AddStatement(SyntaxNode statement)
    {
        scopes.Peek().statements.Add(statement);

        return statement;
    }

    public static void EmitCode(string instr = null)
    {
        sw.WriteLine(instr);
    }

    public static void EmitCode(string instr, params object[] args)
    {
        sw.WriteLine(instr, args);
    }

    public static string NextLabel()
    {
        return string.Format("IL_{0:x}", labelNum++);
    }

    public static void AddLabel(string label)
    {
        sw.WriteLine("{0}: nop", label);
    }

    private static StreamWriter sw;

    private static void GenCode()
    {
        GenProlog();
        scopes.Peek().GenCode();
        GenEpilog();
    }

    private static void GenProlog()
    {
        EmitCode(".assembly extern mscorlib { }");
        EmitCode(".assembly calculator { }");
        EmitCode(".method static void main()");
        EmitCode("{");
        EmitCode(".entrypoint");
        EmitCode(".try");
        EmitCode("{");
        EmitCode();

        EmitCode("// prolog");

        foreach(var variable in variables)
        {
            switch (variable.Value)
            {
                case CType.Bool:
                    EmitCode($".locals init ( bool _{variable.Key} )");
                    break;
                case CType.Int:
                    EmitCode($".locals init ( int32 _{variable.Key} )");
                    break;
                case CType.Double:
                    EmitCode($".locals init ( float64 _{variable.Key} )");
                    break;
            }
        }

        EmitCode();
    }

    private static void GenEpilog()
    {
        EmitCode("IL_0: nop");
        EmitCode("leave EndMain");
        EmitCode("}");
        EmitCode("catch [mscorlib]System.Exception");
        EmitCode("{");
        EmitCode("callvirt instance string [mscorlib]System.Exception::get_Message()");
        EmitCode("call void [mscorlib]System.Console::WriteLine(string)");
        EmitCode("leave EndMain");
        EmitCode("}");
        EmitCode("EndMain: ret");
        EmitCode("}");
    }
}

public enum CType
{
    Int,
    Double,
    Bool,
}

public abstract class SyntaxNode
{
    public virtual CType CheckType() => throw new ErrorException("Expected expression, but got statement");
    public abstract void GenCode();

    protected void EmitCode(string instr = null) =>
        Compiler.EmitCode(instr);

    protected static void EmitCode(string instr, params object[] args) =>
        Compiler.EmitCode(instr, args);
}

public class ScopeNode : SyntaxNode
{
    public List<SyntaxNode> statements = new List<SyntaxNode>();

    public ScopeNode() { }

    public override void GenCode()
    {
        foreach (var statement in ((IEnumerable<SyntaxNode>)statements).Reverse())
        {
            statement.GenCode();
        }
    }
}

public class SemicolonNode : SyntaxNode
{
    private SyntaxNode expression;

    public SemicolonNode(SyntaxNode exp)
    {
        exp.CheckType();
        expression = exp;
    }

    public override void GenCode()
    {
        expression.GenCode();
        EmitCode("pop");
    }
}

public class ReturnNode : SyntaxNode
{
    public override void GenCode()
    {
        EmitCode("br IL_0");
    }
}

public class IfNode : SyntaxNode
{
    private string label;
    private SyntaxNode condition;
    private SyntaxNode statement;

    public IfNode(SyntaxNode con, SyntaxNode stmnt)
    {
        if (con.CheckType() != CType.Bool)
        {
            new Error("If condition must return bool");
        }

        condition = con;
        statement = stmnt;
        label = Compiler.NextLabel();
    }

    public override void GenCode()
    {
        condition.GenCode();
        EmitCode("brfalse {0}", label);
        statement.GenCode();
        Compiler.AddLabel(label);
    }
}

public class IfElseNode : SyntaxNode
{
    private string ifLabel;
    private string elseLabel;
    private SyntaxNode condition;
    private SyntaxNode ifStatement;
    private SyntaxNode elseStatement;

    public IfElseNode(
        SyntaxNode con,
        SyntaxNode ifStmnt,
        SyntaxNode elseStmnt)
    {
        if (con.CheckType() != CType.Bool)
        {
            new Error("If condition must return bool");
        }

        condition = con;
        ifStatement = ifStmnt;
        elseStatement = elseStmnt;
        ifLabel = Compiler.NextLabel();
        elseLabel = Compiler.NextLabel();
    }

    public override void GenCode()
    {
        condition.GenCode();
        EmitCode("brfalse {0}", elseLabel);
        ifStatement.GenCode();
        EmitCode("br {0}", ifLabel);
        Compiler.AddLabel(elseLabel);
        elseStatement.GenCode();
        Compiler.AddLabel(ifLabel);
    }
}

public class WhileNode : SyntaxNode
{
    private string startLabel;
    private string endLabel;
    private SyntaxNode condition;
    private SyntaxNode statement;

    public WhileNode(SyntaxNode con, SyntaxNode stmnt)
    {
        if (con.CheckType() != CType.Bool)
        {
            new Error("While condition must return bool");
        }

        condition = con;
        statement = stmnt;
        startLabel = Compiler.NextLabel();
        endLabel = Compiler.NextLabel();
    }

    public override void GenCode()
    {
        Compiler.AddLabel(startLabel);
        condition.GenCode();
        EmitCode("brfalse {0}", endLabel);
        statement.GenCode();
        EmitCode("br {0}", startLabel);
        Compiler.AddLabel(endLabel);
    }
}

public class ConstBoolLeaf : SyntaxNode
{
    bool val;
    public ConstBoolLeaf(bool v) { val = v; }

    public override CType CheckType() => CType.Bool;
    public override void GenCode()
    {
        EmitCode("ldc.i4 {0}", val ? 1 : 0);
    }
}

public class ConstIntLeaf : SyntaxNode
{
    int val;
    public ConstIntLeaf(int v) { val = v; }

    public override CType CheckType() => CType.Int;
    public override void GenCode()
    {
        EmitCode("ldc.i4 {0}", val);
    }
}

public class ConstDoubleLeaf : SyntaxNode
{
    double val;
    public ConstDoubleLeaf(double v) { val = v; }

    public override CType CheckType() => CType.Double;
    public override void GenCode()
    {
        EmitCode(string.Format(System.Globalization.CultureInfo.InvariantCulture, "ldc.r8 {0}", val));
    }
}

public class IdentLeaf : SyntaxNode
{
    private string id;
    public IdentLeaf(string s)
    {
        if (!Compiler.variables.ContainsKey(s))
        {
            throw new ErrorException($"Attempted to resolve undeclared variable {s}");
        }

        id = s;
    }

    public override CType CheckType() => Compiler.variables[id];

    public override void GenCode()
    {
        Compiler.EmitCode("ldloc _{0}", id);
    }
}

public class DeclIdentNode : SyntaxNode
{
    string id;
    CType type;

    public DeclIdentNode(string i, CType t)
    {
        if (Compiler.variables.ContainsKey(i))
        {
            new Error($"Variable {i} already declared");
        }
        else
        {
            Compiler.variables.Add(i, t);
        }

        type = t;
        id = i;
    }

    public override void GenCode()
    {
        switch (type)
        {
            case CType.Int:
                EmitCode("ldc.i4 {0}", 0);
                EmitCode("stloc _{0}", id);
                break;
            case CType.Double:
                EmitCode("ldc.r8 {0}", 0.0d);
                EmitCode("stloc _{0}", id);
                break;
            case CType.Bool:
                EmitCode("ldc.i4 {0}", 0);
                EmitCode("stloc _{0}", id);
                break;
        }
    }
}

public class WriteNode : SyntaxNode
{
    private SyntaxNode exp;
    public WriteNode(SyntaxNode e) { exp = e; }

    public override void GenCode()
    {
        var t = exp.CheckType();
        if (t == CType.Double)
        {
            EmitCode("call class [mscorlib]System.Globalization.CultureInfo [mscorlib]System.Globalization.CultureInfo::get_InvariantCulture()");
            EmitCode("ldstr \"{0:0.000000}\"");
        }

        exp.GenCode();

        string type = "";
        switch (t)
        {
            case CType.Bool:
                type = "bool";
                break;
            case CType.Int:
                type = "int32";
                break;
            case CType.Double:
                type = "float64";
                EmitCode("box [mscorlib]System.Double");
                EmitCode("call string [mscorlib]System.String::Format(class [mscorlib]System.IFormatProvider, string, object)");
                EmitCode("call void [System.Console]System.Console::Write(string)");
                return;
        }

        EmitCode("call void [System.Console]System.Console::Write({0})", type);
    }
}

public class WriteStrNode : SyntaxNode
{
    private string str;

    public WriteStrNode(string s)
    {
        str = s;
    }

    public override void GenCode()
    {
        EmitCode("ldstr {0}", str);
        EmitCode("call void [System.Console]System.Console::Write(string)");
    }
}

public class ReadNode : SyntaxNode
{
    private string id;

    public ReadNode(string i)
    {
        if (!Compiler.variables.ContainsKey(i))
        {
            new Error($"Attempted to assign to undeclared variable {i}");
        }

        id = i;
    }

    public override void GenCode()
    {
        string t1 = "", t2 = "";

        EmitCode("call string [System.Console]System.Console::ReadLine()");

        switch (Compiler.variables[id])
        {
            case CType.Int:
                t1 = "int32";
                t2 = "Int32";
                EmitCode("call {0} [mscorlib]System.{1}::Parse(string)", t1, t2);
                break;
            case CType.Bool:
                t1 = "bool";
                t2 = "Boolean";
                EmitCode("call {0} [mscorlib]System.{1}::Parse(string)", t1, t2);
                break;
            case CType.Double:
                t1 = "float64";
                t2 = "Double";
                EmitCode("call class [mscorlib]System.Globalization.CultureInfo [mscorlib]System.Globalization.CultureInfo::get_InvariantCulture()");
                EmitCode("call float64 [mscorlib]System.Double::Parse(string, class [mscorlib]System.IFormatProvider)");
                break;
        }

        EmitCode("stloc _{0}", id);
    }
}

class AssignNode : SyntaxNode
{
    private string id;
    private SyntaxNode exp;

    public AssignNode(string i, SyntaxNode e)
    {
        if (!Compiler.variables.ContainsKey(i))
        {
            throw new ErrorException($"Attempted to assign to undeclared variable {i}");
        }

        var idType = Compiler.variables[i];
        var expType = e.CheckType();

        if (idType != expType && !(idType == CType.Double && expType == CType.Int))
        {
            new Error($"Cannot assign {expType} to {idType}");
        }

        id = i;
        exp = e;
    }

    public override CType CheckType()
    {
        var idType = Compiler.variables[id];
        var expType = exp.CheckType();

        if (idType != expType && !(idType == CType.Double && expType == CType.Int))
        {
            new Error($"Cannot assign {expType} to {idType}");
        }

        return idType;
    }

    public override void GenCode()
    {
        exp.GenCode();

        if (Compiler.variables[id] == CType.Double && exp.CheckType() == CType.Int)
        {
            EmitCode("conv.r8");
        }
        EmitCode("stloc _{0}", id);
        EmitCode("ldloc _{0}", id);
    }
}

public abstract class BinaryOperationNode : SyntaxNode
{
    protected string operation;
    protected SyntaxNode exp1;
    protected SyntaxNode exp2;

    public BinaryOperationNode(SyntaxNode ex1, SyntaxNode ex2, string op)
    {
        operation = op;
        exp1 = ex1;
        exp2 = ex2;
    }

    public override CType CheckType()
    {
        var t1 = exp1.CheckType();
        var t2 = exp2.CheckType();

        return t1 == CType.Double || t2 == CType.Double
            ? CType.Double
            : t1;
    }

    public override void GenCode()
    {
        var type1 = exp1.CheckType();
        var type2 = exp2.CheckType();

        exp1.GenCode();
        if (type2 == CType.Double && type1 != CType.Double)
        {
            EmitCode("conv.r8");
        }

        exp2.GenCode();
        if (type1 == CType.Double && type2 != CType.Double)
        {
            EmitCode("conv.r8");
        }

        EmitCode(operation);
    }
}

public class BitOrNode : SyntaxNode
{
    SyntaxNode exp1;
    SyntaxNode exp2;

    public BitOrNode(SyntaxNode ex1, SyntaxNode ex2)
    {
        if (ex1.CheckType() != CType.Int
            || ex2.CheckType() != CType.Int)
        {
            new Error("Attempted bit sum on a non int type");
        }

        exp1 = ex1;
        exp2 = ex2;
    }

    public override CType CheckType()
    {
        return CType.Int;
    }

    public override void GenCode()
    {
        exp1.GenCode();
        exp2.GenCode();
        EmitCode("or");
    }
}

public class BitAndNode : SyntaxNode
{
    SyntaxNode exp1;
    SyntaxNode exp2;

    public BitAndNode(SyntaxNode ex1, SyntaxNode ex2)
    {
        if (ex1.CheckType() != CType.Int
            || ex2.CheckType() != CType.Int)
        {
            new Error("Attempted bit product on a non int type");
        }

        exp1 = ex1;
        exp2 = ex2;
    }

    public override CType CheckType()
    {
        return CType.Int;
    }

    public override void GenCode()
    {
        exp1.GenCode();
        exp2.GenCode();
        EmitCode("and");
    }
}

public abstract class CalculationNode : BinaryOperationNode
{
    public CalculationNode(SyntaxNode ex1, SyntaxNode ex2, string op, string opName)
        : base(ex1, ex2, op)
    {
        if (ex1.CheckType() == CType.Bool
            || ex2.CheckType() == CType.Bool)
        {
            new Error($"Attempted {opName} on a non number type");
        }
    }
}

public class AdditionNode : CalculationNode
{
    public AdditionNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "add", "addition")
    { }
}

public class SubtractionNode : CalculationNode
{
    public SubtractionNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "sub", "subtraction")
    { }
}

public class MultiplicationNode : CalculationNode
{
    public MultiplicationNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "mul", "multiplication")
    { }
}

public class DivisionNode : CalculationNode
{
    public DivisionNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "div", "division")
    { }
}

public abstract class LogicalNode : BinaryOperationNode
{
    protected string label;

    public LogicalNode(SyntaxNode ex1, SyntaxNode ex2, string op, string opName)
        : base(ex1, ex2, op)
    {
        if (ex1.CheckType() != CType.Bool
            || ex2.CheckType() != CType.Bool)
        {
            new Error($"Attempted {opName} on a non bool type");
        }

        label = Compiler.NextLabel();
    }

    public override CType CheckType()
    {
        return CType.Bool;
    }
}

public class OrNode : LogicalNode
{
    public OrNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "or", "bool or")
    { }

    public override void GenCode()
    {
        exp1.GenCode();
        EmitCode("dup");
        EmitCode("brtrue {0}", label);
        exp2.GenCode();
        EmitCode(operation);
        Compiler.AddLabel(label);
    }
}

public class AndNode : LogicalNode
{
    public AndNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "and", "bool and")
    { }

    public override void GenCode()
    {
        exp1.GenCode();
        EmitCode("dup");
        EmitCode("brfalse {0}", label);
        exp2.GenCode();
        EmitCode(operation);
        Compiler.AddLabel(label);
    }
}

public abstract class RelationNode : BinaryOperationNode
{
    public RelationNode(
        SyntaxNode ex1,
        SyntaxNode ex2,
        string op,
        string opName,
        bool canBeBool)
        : base(ex1, ex2, op)
    {
        var t1 = ex1.CheckType();
        var t2 = ex2.CheckType();

        if (canBeBool)
        {
            if (t1 != t2
            && !(t1 == CType.Double && t2 == CType.Int)
            && !(t1 == CType.Int && t2 == CType.Double))
            {
                new Error($"Attempted {opName} check on a {t1} and {t2}");
            }
        }
        else
        {
            if (t1 == CType.Bool || t2 == CType.Bool)
            {
                new Error($"Attempted {opName} check on a non number type");
            }
        }
    }

    public override CType CheckType()
    {
        return CType.Bool;
    }
}

public class EqualityNode : RelationNode
{
    public EqualityNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "ceq", "equality", true)
    { }
}

public class NotEqualityNode : RelationNode
{
    public NotEqualityNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "ceq", "unequality", true)
    { }

    public override void GenCode()
    {
        base.GenCode();
        EmitCode("ldc.i4 0");
        EmitCode("ceq");
    }
}

public class GreaterNode : RelationNode
{
    public GreaterNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "cgt", "greater", false)
    { }
}

public class LessNode : RelationNode
{
    public LessNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "clt", "less", false)
    { }
}

public class GreaterOrEqualNode : RelationNode
{
    public GreaterOrEqualNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "clt", "greater or equal", false)
    { }

    public override void GenCode()
    {
        base.GenCode();
        EmitCode("ldc.i4 0");
        EmitCode("ceq");
    }
}

public class LessOrEqualNode : RelationNode
{
    public LessOrEqualNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "cgt", "less or equal", false)
    { }

    public override void GenCode()
    {
        base.GenCode();
        EmitCode("ldc.i4 0");
        EmitCode("ceq");
    }
}

public class IntConversionNode : SyntaxNode
{
    private SyntaxNode exp;

    public IntConversionNode(SyntaxNode ex)
    {
        exp = ex;
    }

    public override CType CheckType()
    {
        return CType.Int;
    }

    public override void GenCode()
    {
        exp.GenCode();

        if (exp.CheckType() == CType.Double)
        {
            EmitCode("conv.i4");
        }
    }
}

public class DoubleConversionNode : SyntaxNode
{
    private SyntaxNode exp;

    public DoubleConversionNode(SyntaxNode ex)
    {
        exp = ex;
    }

    public override CType CheckType()
    {
        return CType.Double;
    }

    public override void GenCode()
    {
        exp.GenCode();

        if (exp.CheckType() != CType.Double)
        {
            EmitCode("conv.r8");
        }
    }
}

public class NegNode : SyntaxNode
{
    private SyntaxNode exp;

    public NegNode(SyntaxNode ex)
    {
        if (ex.CheckType() == CType.Bool)
        {
            new Error("Attempted negation on a non number type");
        }

        exp = ex;
    }

    public override CType CheckType()
    {
        return exp.CheckType();
    }

    public override void GenCode()
    {
        exp.GenCode();
        EmitCode("neg");
    }
}

public class BitNegNode : SyntaxNode
{
    private SyntaxNode exp;

    public BitNegNode(SyntaxNode ex)
    {
        if (ex.CheckType() != CType.Int)
        {
            new Error("Attempted bit negation on a non int type");
        }

        exp = ex;
    }

    public override CType CheckType()
    {
        return CType.Int;
    }

    public override void GenCode()
    {
        exp.GenCode();
        EmitCode("not");
    }
}

public class BoolNegNode : SyntaxNode
{
    private SyntaxNode exp;

    public BoolNegNode(SyntaxNode ex)
    {
        if (ex.CheckType() != CType.Bool)
        {
            new Error("Attempted logical negation on a non bool type");
        }

        exp = ex;
    }

    public override CType CheckType()
    {
        return CType.Bool;
    }

    public override void GenCode()
    {
        exp.GenCode();
        EmitCode("ldc.i4 0");
        EmitCode("ceq");
    }
}

public class Error
{
    public Error() 
    { 
        ++Compiler.errors;
    }

    public Error(string msg) 
    { 
        ++Compiler.errors;
        Console.WriteLine($"Line {Scanner.lineno}: {msg}");
    }
}

public class ErrorException : Exception
{
    public ErrorException(string msg)
    {
        ++Compiler.errors;
        Console.WriteLine($"Line {Scanner.lineno}: {msg}");
    }
}
