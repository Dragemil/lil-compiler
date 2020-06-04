using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class CalculationNode : BinaryOperationNode
{
    public CalculationNode(SyntaxNode ex1, SyntaxNode ex2, string op, string opName)
        : base(ex1, ex2, op)
    {
        if (ex1.CheckType() == CType.Bool
            || ex2.CheckType() == CType.Bool)
        {
            new Error($"Attempted {opName} on a non int type");
        }
    }
}

public class AdditionNode : CalculationNode
{
    public AdditionNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "add", "addition")
    { }
}

public class SubtractionNode : CalculationNode
{
    public SubtractionNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "sub", "subtraction")
    { }
}

public class MultiplicationNode : CalculationNode
{
    public MultiplicationNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "mul", "multiplication")
    { }
}

public class DivisionNode : CalculationNode
{
    public DivisionNode(SyntaxNode ex1, SyntaxNode ex2)
        : base(ex1, ex2, "div", "division")
    { }
}