using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

