using MathTools;
using MathTools.Internal;
internal class Program
{
    private static void Main(string[] args)
    {
        new Expression("").TEST();
        new Equation("", "").TEST();
        
        if (args.Length == 0) InputTest();
        else {
            Console.WriteLine(new Expression(args[0]).Simplify());
            Console.WriteLine(new Equation(args[1]).Solve());
        }
        Console.ReadLine();
    }

    private static void InputTest()
    {
        Console.Write("Enter an Expression: ");
        Console.WriteLine("Evaluated: " + new Expression(Console.ReadLine()!).Simplify());

        Console.Write("Enter an Equation: ");
        Console.WriteLine("Solved to: " + new Equation(Console.ReadLine()!).Solve());
    }
}