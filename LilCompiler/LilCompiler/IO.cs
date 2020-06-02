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
        exp.GenCode();

        string type = "";
        switch (exp.CheckType())
        {
            case CType.Bool:
                type = "bool";
                break;
            case CType.Int:
                type = "int32";
                break;
            case CType.Double:
                type = "float64";
                break;
        }

        EmitCode("call void [System.Console]System.Console::WriteLine({0})", type);
    }
}

