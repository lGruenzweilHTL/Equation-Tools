using static Master;
namespace MathTools.Internal;

public class Equation
{
    public Equation(string left, string right)
    {
        this.left = left;
        this.right = right;
    }
    public Equation(string s)
    {
        var sp = s.Split('=');

        if (sp.Length != 2) throw new ArgumentException("Too many/not enough '=' signs");

        left = sp[0];
        right = sp[1];
    }

    public string left;
    public string right;
    public int GetUniqueVariableCount(string s)
    {
        int uniques = 0;
        List<char> alreadyListed = new();

        foreach (char c in s)
        {
            if (allowedVariableNames.Contains(c) && !alreadyListed.Contains(c))
            {
                uniques++;
                alreadyListed.Add(c);
            }
        }

        return uniques;
    }

    public string Solve()
    {
        try
        {
            return Solve(left, right);
        }
        catch { throw; }
    }

    public void TEST()
    {
        Console.WriteLine("TEST CASES (equation):");
        Console.WriteLine("Expected: x=0; Solved: " + new Equation("2*x + x = x + x").Solve());
        Console.WriteLine("Expected: x=1; Solved: " + new Equation("3x = 3").Solve());
        Console.WriteLine("Expected: infinite solutions; Solved: " + new Equation("4*4 = 16").Solve());
        Console.WriteLine("Expected: no solution; Solved: " + new Equation("4*5 = 5*5").Solve());
        Console.WriteLine("Expected: x=-0,5; Solved: " + new Equation("1 - (x + 2) = x").Solve());
        Console.WriteLine("Expected: x=-2,5; Solved: " + new Equation("2(x+2) - x = -(x+1)").Solve());
        Console.WriteLine("Expected: x=243; Solved: " + new Equation("(1+2)^5=x").Solve());
        Console.WriteLine("Expected: infinite solutions; Solved: " + new Equation("x=x").Solve());
        Console.WriteLine("Expected: a=1; Evaluated: " + new Equation("125^(1/3) = 5a").Solve());
        Console.WriteLine("Expected: y=5; Solved: " + new Equation("y^2 + y = y^2 + 5").Solve());
        Console.WriteLine("\n");
    }

    private string Solve(string left, string right)
    {
        //* Simplify both sides individually
        left = new Expression(left).Simplify();
        right = new Expression(right).Simplify();

        if (GetUniqueVariableCount(left + right) > 1) throw new ArgumentException("Too many variables in equation");

        // Early return if no variables
        if (GetUniqueVariableCount(left + right) == 0)
        {
            if (left == right) return infiniteSolutionsText;
            else return noSolutionsText;
        }

        // Add '+' to the start for easier handling (if there's not already a '-')
        if (left[0] != '-') left = left.Insert(0, "+");
        if (right[0] != '-') right = right.Insert(0, "+");

        (left, right) = MoveVariable(left, right);
        if (right == "") right = "0";

        //* Simplify both sides individually
        left = new Expression(left).Simplify();
        right = new Expression(right).Simplify();

        if (left == "") left = "0";

        if (left == "0" && right != "0") return noSolutionsText;
        if (left == right) return infiniteSolutionsText;

        (left, right) = DivideVariables(left, right);

        return $"{left}={right}";
    }
    private (string left, string right) MoveVariable(string left, string right)
    {
        char variable = (left + right).First(c => allowedVariableNames.Contains(c));

        //* always move variable to the left side
        if (right.Contains(variable))
        {
            int index = right.IndexOf(variable);
            string product = GetProductAround(right, index);

            // remove product from right side
            right = right.Remove(Math.Max(0, index - product.Length + 1), product.Length);

            // find out operator, then cut from product
            char op = product[0];
            product = product[1..];

            // add product to left side
            char leftOp = op == '+' ? '-' : '+'; // invert operator for left side
            left += leftOp + product;
        }

        //* always move numbers to the right side
        for (int i = 0; i < left.Length; i++)
        {
            string product = GetProductAround(left, i);
            if (product == "" || product.Any(c => allowedVariableNames.Contains(c))) continue;

            // remove product from left side
            left = left.Remove(i - 1, product.Length);

            // find out operator, then cut from product
            char op = product[0];
            product = product[1..];

            // add product to right side
            char rightOp = op == '+' ? '-' : '+'; // invert operator for right side
            right += rightOp + product;

            // continue on next product
            i += product.Length;
        }

        return (left, right);
    }
    private string GetProductAround(string s, int index)
    {
        if (s[index] is '+' or '-') return "";

        int startIndex = 0;
        int endIndex = s.Length;

        for (int i = index - 1; i >= 0; i--)
        {
            if (s[i] is '+' or '-')
            {
                startIndex = i; // include the '+' or '-' for further handling
                break;
            }
        }

        for (int i = index; i < s.Length; i++)
        {
            if (s[i] is '+' or '-')
            {
                endIndex = i;
                break;
            }
        }

        return s[startIndex..endIndex];
    }

    private (string left, string right) DivideVariables(string left, string right)
    {
        if (left.Contains('^')) (left, right) = EvalPowers(left, right);
        if (left.All(c => allowedVariableNames.Contains(c))) return (left, right);

        char variable = (left + right).First(c => allowedVariableNames.Contains(c));
        int variableIndex = left.IndexOf(variable);

        string number = left[..variableIndex];
        if (number == "-") number = "-1";

        left = variable.ToString();
        right = $"({right})/{number}";

        //* Simplify both sides individually
        left = new Expression(left.Replace(" ", "")).Simplify();
        right = new Expression(right.Replace(" ", "")).Simplify();

        return (left, right);
    }
    private (string left, string right) EvalPowers(string left, string right)
    {
        char variable = (left + right).First(c => allowedVariableNames.Contains(c));
        int powerIndex = left.IndexOf(variable) + 2;

        double power = double.Parse(left[powerIndex..]);

        left = left[..(powerIndex - 1)];
        right = new Expression($"{right}^(1/{power})").Simplify();

        return (left, right);
    }
}