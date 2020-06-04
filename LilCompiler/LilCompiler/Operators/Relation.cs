using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        EmitCode("not");
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
        EmitCode("not");
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
        EmitCode("not");
    }
}
