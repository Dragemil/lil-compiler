using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

public abstract class LogicalNode : BinaryOperationNode
{
    protected string label;

    public LogicalNode(SyntaxNode ex1, SyntaxNode ex2, string op, string opName)
        : base(ex1, ex2, op)
    {
        if (ex1.CheckType() != CType.Bool
            || ex2.CheckType() != CType.Bool)
        {
            new Error($"Attempted {opName} on a non bool type");
        }

        label = Compiler.NextLabel();
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

    public override void GenCode()
    {
        exp1.GenCode();
        EmitCode("dup");
        EmitCode("brtrue {0}", label);
        exp2.GenCode();
        EmitCode(operation);
        Compiler.AddLabel(label);
    }
}

public class AndNode : LogicalNode
{
    public AndNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "and", "bool and")
    { }

    public override void GenCode()
    {
        exp1.GenCode();
        EmitCode("dup");
        EmitCode("brfalse {0}", label);
        exp2.GenCode();
        EmitCode(operation);
        Compiler.AddLabel(label);
    }
}
