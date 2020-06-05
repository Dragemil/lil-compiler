
using System;
using System.IO;
using System.Collections.Generic;
using GardensPoint;

public class Compiler
{
    public static int errors = 0;

    public static Stack<ScopeNode> scopes = new Stack<ScopeNode>(new[] { new ScopeNode() });

    public static Dictionary<string, CType> variables =
        new Dictionary<string, CType>();

    private static int labelNum = 0;
    private static string label = null;

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

    public static void AddStatement(SyntaxNode statement)
    {
        scopes.Peek().statements.Add(statement);

        if (statement is ScopeNode scope)
        {
            scopes.Push(scope);
        }
    }

    public static void EmitCode(string instr = null)
    {
        if (label is string lbl)
        {
            sw.WriteLine($"{lbl}: {instr}");
            label = null;
        }
        else
        {
            sw.WriteLine(instr);
        }
    }

    public static void EmitCode(string instr, params object[] args)
    {
        if (label is string lbl)
        {
            sw.WriteLine($"{lbl}: {instr}", args);
            label = null;
        }
        else
        {
            sw.WriteLine(instr, args);
        }
    }

    public static string NextLabel()
    {
        return string.Format("IL_{0:x}", labelNum++);
    }

    public static void AddLabel(string label)
    { Compiler.label = label; }

    private static StreamWriter sw;

    private static void GenCode()
    {
        GenProlog();

        try
        {
            scopes.Peek().GenCode();
        }
        catch (ReturnException)
        { }
        
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
        
        EmitCode(".locals init ( float64 ftemp )");
        EmitCode(".locals init ( bool btemp )");

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

public class Variable
{
    public bool assigned = false;
    public readonly CType type;

    public Variable(CType t) { type = t; }
}

public abstract class SyntaxNode
{
    public virtual CType CheckType() => throw new ErrorException("Type checked on a statement");
    public abstract void GenCode();

    protected void EmitCode(string instr = null) =>
        Compiler.EmitCode(instr);

    protected static void EmitCode(string instr, params object[] args) =>
        Compiler.EmitCode(instr, args);
}

public class ScopeNode : SyntaxNode
{
    public List<SyntaxNode> statements = new List<SyntaxNode>();

    public override void GenCode()
    {
        foreach (var statement in statements)
        {
            statement.GenCode();
        }
    }
}

public class SemicolonNode : SyntaxNode
{
    public override void GenCode()
    {
        EmitCode("pop");
    }
}

public class ReturnNode : SyntaxNode
{
    public override void GenCode()
    { throw new ReturnException(); }
}

public class ReturnException : Exception
{ }

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
