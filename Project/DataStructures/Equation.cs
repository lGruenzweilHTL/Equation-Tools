using static Master;

namespace MathTools;

public class Equation {
    public Equation(string left, string right) {
        this.Left = left;
        this.Right = right;
    }

    public Equation(string s) {
        string[] sp = s.Split('=');

        if (sp.Length != 2) throw new ArgumentException("Too many/not enough '=' signs");

        Left = sp[0];
        Right = sp[1];
    }

    public string Left { get; private set; }
    public string Right { get; private set; }

    public int GetUniqueVariableCount(string s) {
        int uniques = 0;
        List<char> alreadyListed = new();

        foreach (char c in s) {
            if (AllowedVariableNames.Contains(c) && !alreadyListed.Contains(c)) {
                uniques++;
                alreadyListed.Add(c);
            }
        }

        return uniques;
    }

    public string Solve() {
        return Solve(Left, Right);
    }

    private string Solve(string left, string right) {
        //* Simplify both sides individually
        left = new Expression(left).Simplify();
        right = new Expression(right).Simplify();

        if (GetUniqueVariableCount(left + right) > 1) throw new ArgumentException("Too many variables in equation");

        // Early return if no variables
        if (GetUniqueVariableCount(left + right) == 0) {
            if (left == right) return InfiniteSolutionsText;
            else return NoSolutionsText;
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

        if (left == "0" && right != "0") return NoSolutionsText;
        if (left == right) return InfiniteSolutionsText;

        (left, right) = DivideVariables(left, right);

        return $"{left}={right}";
    }

    private (string left, string right) MoveVariable(string left, string right) {
        char variable = (left + right).First(c => AllowedVariableNames.Contains(c));

        //* always move variable to the left side
        if (right.Contains(variable)) {
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
        for (int i = 0; i < left.Length; i++) {
            string product = GetProductAround(left, i);
            if (product == "" || product.Any(c => AllowedVariableNames.Contains(c))) continue;

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

    private string GetProductAround(string s, int index) {
        if (s[index] is '+' or '-') return "";

        int startIndex = 0;
        int endIndex = s.Length;

        for (int i = index - 1; i >= 0; i--) {
            if (s[i] is '+' or '-') {
                startIndex = i; // include the '+' or '-' for further handling
                break;
            }
        }

        for (int i = index; i < s.Length; i++) {
            if (s[i] is '+' or '-') {
                endIndex = i;
                break;
            }
        }

        return s[startIndex..endIndex];
    }

    private (string left, string right) DivideVariables(string left, string right) {
        if (left.Contains('^')) (left, right) = EvalPowers(left, right);
        if (left.All(c => AllowedVariableNames.Contains(c))) return (left, right);

        char variable = (left + right).First(c => AllowedVariableNames.Contains(c));
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

    private (string left, string right) EvalPowers(string left, string right) {
        char variable = (left + right).First(c => AllowedVariableNames.Contains(c));
        int powerIndex = left.IndexOf(variable) + 2;

        double power = double.Parse(left[powerIndex..]);

        left = left[..(powerIndex - 1)];
        right = new Expression($"{right}^(1/{power})").Simplify();

        return (left, right);
    }
}