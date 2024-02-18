internal class Program
{
    private static void Main()
    {
        Calculation c = new("-2*3^2+5");
        if (c.TryEvaluate(out string result)) Console.WriteLine(result);
        Console.ReadLine();
    }
}