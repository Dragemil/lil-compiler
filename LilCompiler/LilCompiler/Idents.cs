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

        if (!Compiler.variables[s].assigned)
        {
            new Error($"Attempted to resolve unassigned variable {s}");
        }

        id = s;
    }

    public override CType CheckType() => Compiler.variables[id].type;

    public override void GenCode()
    {
        Compiler.EmitCode("ldloc _{0}", id);
    }
}

public class DeclIdentNode : SyntaxNode
{
    public DeclIdentNode(string id, CType type)
    {
        if (Compiler.variables.ContainsKey(id))
        {
            new Error($"Variable {id} already declared");
        }

        Compiler.variables.Add(id, new Variable(type));
    }

    public override void GenCode()
    { }
}

