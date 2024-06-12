using MathTools;
using MathTools.Internal;
internal class Program {
    private static void Main(string[] args) {
        Console.WriteLine(new Expression("1/(x+1)").Simplify());
        do {
            Console.Write("Enter an equation (invalid equation for exit): ");
            string equation = Console.ReadLine();
            try {
                Console.WriteLine("Solved to: {0}\n", new Equation(equation).Solve());
            }
            catch (ArgumentException) {
                break;
            }
        } while (true);
    }
}