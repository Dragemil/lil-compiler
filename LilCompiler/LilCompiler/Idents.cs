using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IdentLeaf : SyntaxNode
{
    private string id;
    public IdentLeaf(string s)
    {
        if (!Compiler.variables.ContainsKey(s))
        {
            new Error($"Attempted to resolve undeclared variable {s}");
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
                EmitCode("ldc.i4 {0}", 0.0d);
                EmitCode("stloc _{0}", id);
                break;
            case CType.Bool:
                EmitCode("ldc.i4 {0}", 0);
                EmitCode("stloc _{0}", id);
                break;
        }
    }
}