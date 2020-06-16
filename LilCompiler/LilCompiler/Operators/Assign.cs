using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

