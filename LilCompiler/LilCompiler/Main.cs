
using System;
using System.IO;
using System.Collections.Generic;
using GardensPoint;
using System.Linq;

public class Compiler
{
    public static int errors = 0;

    public static Stack<ScopeNode> scopes = new Stack<ScopeNode>(new[] { new ScopeNode() });

    public static Dictionary<string, CType> variables =
        new Dictionary<string, CType>();

    private static int labelNum = 1;

    // arg[0] określa plik źródłowy
    // pozostałe argumenty są ignorowane
    public static int Main(string[] args)
    {
        string file;
        FileStream source;
        Console.WriteLine("\nDouble-Pass CIL Code Generator for Mini Language - Gardens Point");
        if (args.Length >= 1)
            file = args[0];
        else
        {
            Console.Write("\nsource file:  ");
            file = Console.ReadLine();
        }
        try
        {
            var sr = new StreamReader(file);
            string str = sr.ReadToEnd();
            sr.Close();
            source = new FileStream(file, FileMode.Open);
        }
        catch (Exception e)
        {
            Console.WriteLine("\n" + e.Message);
            return 1;
        }

        Scanner scanner = new Scanner(source);
        Parser parser = new Parser(scanner);
        try
        {
            Console.WriteLine();
            if (!parser.Parse())
            {
                throw new ErrorException("Invalid syntax");
            }
        }
        catch (ErrorException) { }

        if (errors == 0)
        {
            sw = new StreamWriter(file + ".il");
            GenCode();
            sw.Close();
            source.Close();
            Console.WriteLine("  compilation successful\n");
        }
        else
        {
            Console.WriteLine($"\n  {errors} errors detected\n");
            File.Delete(file + ".il");
        }
        return errors == 0 ? 0 : 2;
    }

    public static SyntaxNode AddStatement(SyntaxNode statement)
    {
        scopes.Peek().statements.Add(statement);

        return statement;
    }

    public static void EmitCode(string instr = null)
    {
        sw.WriteLine(instr);
    }

    public static void EmitCode(string instr, params object[] args)
    {
        sw.WriteLine(instr, args);
    }

    public static string NextLabel()
    {
        return string.Format("IL_{0:x}", labelNum++);
    }

    public static void AddLabel(string label)
    {
        sw.WriteLine("{0}: nop", label);
    }

    private static StreamWriter sw;

    private static void GenCode()
    {
        GenProlog();
        scopes.Peek().GenCode();
        GenEpilog();
    }

    private static void GenProlog()
    {
        EmitCode(".assembly extern mscorlib { }");
        EmitCode(".assembly calculator { }");
        EmitCode(".method static void main()");
        EmitCode("{");
        EmitCode(".entrypoint");
        EmitCode(".try");
        EmitCode("{");
        EmitCode();

        EmitCode("// prolog");

        foreach(var variable in variables)
        {
            switch (variable.Value)
            {
                case CType.Bool:
                    EmitCode($".locals init ( bool _{variable.Key} )");
                    break;
                case CType.Int:
                    EmitCode($".locals init ( int32 _{variable.Key} )");
                    break;
                case CType.Double:
                    EmitCode($".locals init ( float64 _{variable.Key} )");
                    break;
            }
        }

        EmitCode();
    }

    private static void GenEpilog()
    {
        EmitCode("IL_0: nop");
        EmitCode("leave EndMain");
        EmitCode("}");
        EmitCode("catch [mscorlib]System.Exception");
        EmitCode("{");
        EmitCode("callvirt instance string [mscorlib]System.Exception::get_Message()");
        EmitCode("call void [mscorlib]System.Console::WriteLine(string)");
        EmitCode("leave EndMain");
        EmitCode("}");
        EmitCode("EndMain: ret");
        EmitCode("}");
    }
}

public enum CType
{
    Int,
    Double,
    Bool,
}

public abstract class SyntaxNode
{
    public virtual CType CheckType() => throw new ErrorException("Expected expression, but got statement");
    public abstract void GenCode();

    protected void EmitCode(string instr = null) =>
        Compiler.EmitCode(instr);

    protected static void EmitCode(string instr, params object[] args) =>
        Compiler.EmitCode(instr, args);
}

public class ScopeNode : SyntaxNode
{
    public List<SyntaxNode> statements = new List<SyntaxNode>();

    public ScopeNode() { }

    public override void GenCode()
    {
        foreach (var statement in ((IEnumerable<SyntaxNode>)statements).Reverse())
        {
            statement.GenCode();
        }
    }
}

public class SemicolonNode : SyntaxNode
{
    private SyntaxNode expression;

    public SemicolonNode(SyntaxNode exp)
    {
        exp.CheckType();
        expression = exp;
    }

    public override void GenCode()
    {
        expression.GenCode();
        EmitCode("pop");
    }
}

public class ReturnNode : SyntaxNode
{
    public override void GenCode()
    {
        EmitCode("br IL_0");
    }
}

public class Error
{
    public Error() 
    { 
        ++Compiler.errors;
    }

    public Error(string msg) 
    { 
        ++Compiler.errors;
        Console.WriteLine($"Line {Scanner.lineno}: {msg}");
    }
}

public class ErrorException : Exception
{
    public ErrorException(string msg)
    {
        ++Compiler.errors;
        Console.WriteLine($"Line {Scanner.lineno}: {msg}");
    }
}
