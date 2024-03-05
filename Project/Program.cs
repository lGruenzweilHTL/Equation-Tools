using MathTools;
using MathTools.Internal;
internal class Program
{
    private static void Main()
    {
        new Expression("").TEST();
        new Equation("", "").TEST();
        InputTest();
        Console.ReadLine();
    }

    private static void InputTest()
    {
        Console.Write("Enter an Equation: ");
        Console.WriteLine("Solved to: " + new Equation(Console.ReadLine()!).Solve());
    }
}