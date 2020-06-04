using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class LogicalNode : BinaryOperationNode
{
    public LogicalNode(SyntaxNode ex1, SyntaxNode ex2, string op, string opName)
        : base(ex1, ex2, op)
    {
        if (ex1.CheckType() != CType.Bool
            || ex2.CheckType() != CType.Bool)
        {
            new Error($"Attempted {opName} on a non bool type");
        }
    }

    public override CType CheckType()
    {
        return CType.Bool;
    }
}

public class OrNode : LogicalNode
{
    public OrNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "or", "bool or")
    { }
}

public class AndNode : LogicalNode
{
    public AndNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "and", "bool and")
    { }
}
