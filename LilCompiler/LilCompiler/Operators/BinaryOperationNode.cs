using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


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

