using System;
using System.IO;
using System.Collections.Generic;

public class ConstBoolLeaf : SyntaxNode
{
    bool val;
    public ConstBoolLeaf(bool v) { val = v; }

    public override CType CheckType() => CType.Bool;
    public override void GenCode()
    {
        EmitCode("ldc.i4 {0}", val ? 1 : 0);
    }
}

public class ConstIntLeaf : SyntaxNode
{
    int val;
    public ConstIntLeaf(int v) { val = v; }

    public override CType CheckType() => CType.Int;
    public override void GenCode()
    {
        EmitCode("ldc.i4 {0}", val);
    }
}

public class ConstDoubleLeaf : SyntaxNode
{
    double val;
    public ConstDoubleLeaf(double v) { val = v; }

    public override CType CheckType() => CType.Double;
    public override void GenCode()
    {
        EmitCode(string.Format(System.Globalization.CultureInfo.InvariantCulture, "ldc.r8 {0}", val));
    }
}