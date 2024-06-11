using MathTools;
using MathTools.Internal;
internal class Program
{
    private static void Main(string[] args)
    {
        if (args.Length != 2) InputTest();
        else {
            Console.WriteLine(new Expression(args[0]).Simplify());
            Console.WriteLine(new Equation(args[1]).Solve());
        }
        Console.ReadLine();
    }

    private static void InputTest()
    {
        Console.Write("Enter an Equation: ");
        Console.WriteLine("Solved to: " + new Equation(Console.ReadLine()!).Solve());
    }
}