internal class Program
{
    private static void Main()
    {
        new Expression("").TEST();
        InputTest();
        Console.ReadLine();
    }

    private static void InputTest()
    {
        Console.Write("Enter an Expression: ");
        Console.WriteLine("Simplified to: " + new Expression(Console.ReadLine()!).Simplify());
    }
}