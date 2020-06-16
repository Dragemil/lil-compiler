using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class IfNode : SyntaxNode
{
    private string label;
    private SyntaxNode condition;
    private SyntaxNode statement;

    public IfNode(SyntaxNode con, SyntaxNode stmnt)
    {
        if (con.CheckType() != CType.Bool)
        {
            new Error("If condition must return bool");
        }

        condition = con;
        statement = stmnt;
        label = Compiler.NextLabel();
    }

    public override void GenCode()
    {
        condition.GenCode();
        EmitCode("brfalse.s {0}", label);
        statement.GenCode();
        Compiler.AddLabel(label);
    }
}

public class IfElseNode : SyntaxNode
{
    private string ifLabel;
    private string elseLabel;
    private SyntaxNode condition;
    private SyntaxNode ifStatement;
    private SyntaxNode elseStatement;

    public IfElseNode(
        SyntaxNode con,
        SyntaxNode ifStmnt,
        SyntaxNode elseStmnt)
    {
        if (con.CheckType() != CType.Bool)
        {
            new Error("If condition must return bool");
        }

        condition = con;
        ifStatement = ifStmnt;
        elseStatement = elseStmnt;
        ifLabel = Compiler.NextLabel();
        elseLabel = Compiler.NextLabel();
    }

    public override void GenCode()
    {
        condition.GenCode();
        EmitCode("brfalse.s {0}", elseLabel);
        ifStatement.GenCode();
        EmitCode("br.s {0}", ifLabel);
        Compiler.AddLabel(elseLabel);
        elseStatement.GenCode();
        Compiler.AddLabel(ifLabel);
    }
}

public class WhileNode : SyntaxNode
{
    private string startLabel;
    private string endLabel;
    private SyntaxNode condition;
    private SyntaxNode statement;

    public WhileNode(SyntaxNode con, SyntaxNode stmnt)
    {
        if (con.CheckType() != CType.Bool)
        {
            new Error("While condition must return bool");
        }

        condition = con;
        statement = stmnt;
        startLabel = Compiler.NextLabel();
        endLabel = Compiler.NextLabel();
    }

    public override void GenCode()
    {
        Compiler.AddLabel(startLabel);
        condition.GenCode();
        EmitCode("brfalse.s {0}", endLabel);
        statement.GenCode();
        EmitCode("br.s {0}", startLabel);
        Compiler.AddLabel(endLabel);
    }
}