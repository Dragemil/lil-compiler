using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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