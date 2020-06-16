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