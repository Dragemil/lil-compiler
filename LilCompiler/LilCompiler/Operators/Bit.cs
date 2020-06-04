using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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