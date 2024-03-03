using System.Text;
using static Master;
using MathTools.Internal;

namespace MathTools;

public class Expression
{
    public Expression(string s)
    {
        current = s;
    }
    public string current;
    public int variableCount => current.Count(c => allowedVariableNames.Contains(c));

    public string Simplify()
    {
        try
        {
            return Simplify(current);
        }
        catch { throw; }
    }

    private string Simplify(string s)
    {
        if (s is null or "") return "";

        s = s.Format();

        if (variableCount == 0) return new Calculation(s).Evaluate();

        s = EvaluateBracketCoefficients(s);
        s = EvaluateBracketSigns(s);
        s = SimplifyValueString(s);

        return s;
    }

    public void TEST()
    {
        Console.WriteLine("TEST CASES (expression):");
        Console.WriteLine("Expected: 2-1-2+3-x+3-2; Evaluated: " + EvaluateBracketSigns("2 - (1 + 2) + (3 - x) - (-3 + 2)"));
        Console.WriteLine("Expected: 1; Evaluated: " + new Expression("2 - (1 + 2) + (3 - 2) - (-3 + 2)").Simplify());
        Console.WriteLine("Expected: 3-x; Evaluated: " + new Expression("2 - (1 + 2) + (3 - x) - (-3 + 2)").Simplify());
        Console.WriteLine("Expected: 2x-4; Evaluated: " + new Expression("2 * (x - 2)").Simplify());
        Console.WriteLine("Expected: 0,5x+1; Evaluated: " + new Expression("(x + 2) / 2").Simplify());
        Console.WriteLine("Expected: x+2; Evaluated: " + new Expression("2 * (x + 2) / 2").Simplify());
        Console.WriteLine("Expected: x^2+2x; Evaluated: " + new Expression("x * (x + 2)").Simplify());
        Console.WriteLine("Expected: 4x+8; Evaluated: " + new Expression("(x + 2) * (1 + 3)").Simplify());
        Console.WriteLine("\n");
    }

    private string EvaluateBracketCoefficients(string s)
    {
        s = s.Replace(" ", "");

        // Early return if there aren't any coefficients to evaluate
        if (!s.Contains('*') && !s.Contains('/')) return s;

        int targetLayer = GetMaxBracketLayer(s);
        while (targetLayer > 0)
        {
            int currentLayer = 0;

            int closedIndex;
            int openIndex = 0;


            for (int i = 0; i < s.Length; i++)
            {
                bool bracketTimesBracket = false;

                if (s[i] == '(') //* find out open- and closedIndex
                {
                    if (++currentLayer == targetLayer) openIndex = i;
                    continue;
                }
                else if (s[i] == ')')
                {
                    if (--currentLayer == targetLayer - 1)
                    {
                        closedIndex = i;
                    }
                    else continue;
                }
                else continue;

                if (s[openIndex] != '(') break; // fix weird case with nested brackets

                bool dotOperationAtStart = false;
                if (openIndex != 0)
                {
                    dotOperationAtStart = s[openIndex - 1] == '*' || s[openIndex - 1] == '/';
                }
                bool dotOperationAtEnd = false;
                if (closedIndex != s.Length - 1)
                {
                    dotOperationAtEnd = s[closedIndex + 1] == '*' || s[closedIndex + 1] == '/';
                }

                if (!dotOperationAtStart && !dotOperationAtEnd)
                {
                    targetLayer--;
                    break;
                }

                string coefficient = "";

                string startProduct = "";
                string endProduct = "";

                if (openIndex > 1 && s[openIndex - 1] is '*' or '/')
                {
                    if (s[openIndex - 2] == ')') { startProduct = GetBracketAt(s, openIndex - 2); bracketTimesBracket = true; }
                    else startProduct = GetStartOperationAt(s, openIndex - 2);
                }
                if (closedIndex < s.Length - 2 && s[closedIndex + 1] is '*' or '/')
                {
                    if (s[closedIndex + 2] == '(') { endProduct = GetBracketAt(s, closedIndex + 2); bracketTimesBracket = true; }
                    else endProduct = GetEndOperationAt(s, closedIndex + 2);
                }

                if (startProduct != "")
                {
                    if (dotOperationAtStart && s[openIndex - 1] == '*')
                    {
                        coefficient += "*" + startProduct;
                    }
                    if (dotOperationAtStart && s[openIndex - 1] == '/')
                    {
                        //TODO : implementation with reworked power system
                    }
                }
                if (endProduct != "")
                {
                    if (dotOperationAtEnd && s[closedIndex + 1] == '*')
                    {
                        coefficient += "*" + endProduct;
                    }
                    if (dotOperationAtEnd && s[closedIndex + 1] == '/')
                    {
                        coefficient += "/" + endProduct;
                    }
                }

                // Moved from inside the loop to outside (only at the end),
                // because this led to some bugs with nested brackets
                // Example of error: "3*((3*x))" would simplify to "27x"
                s = s.Insert(closedIndex, coefficient);
                for (int j = closedIndex - 1; j >= openIndex; j--)
                {
                    if (s[j] is '+' or '-')
                    {
                        s = s.Insert(j, coefficient);
                    }
                }

                if (startProduct != "") s = s.Remove(openIndex - startProduct.Length - 1, startProduct.Length + 1);

                // Find new closedIndex (because of insertions and deletions)
                closedIndex = FindNewClosedIndex(s, targetLayer, 0);

                if (endProduct != "") s = s.Remove(closedIndex + 1, endProduct.Length + 1);

                //* if this was the last operation on this layer, decrement to work on the next one
                bool lastOpOnLayer = s[closedIndex..].Count(c => c == ')') == targetLayer;
                if (!bracketTimesBracket) { if (lastOpOnLayer) targetLayer--; }
                else targetLayer++;

                i = closedIndex + 1;
            }
        }

        return s;
    }
    private int FindNewClosedIndex(string s, int targetLayer, int startIndex)
    {
        int layer = 0;
        for (int j = startIndex; j < s.Length; j++)
        {
            if (s[j] == '(') layer++;
            if (s[j] == ')')
            {
                if (layer == targetLayer)
                {
                    return j;
                }
                layer--;
            }
        }
        return -1;
    }

    private string GetEndOperationAt(string s, int index)
    {
        int startIndex = 0;
        int endIndex = s.Length;

        for (int i = index - 1; i >= 0; i--) // get startIndex
        {
            if (s[i] is '+' or '-' or '(' or ')')
            {
                startIndex = i + 2;
                break;
            }
        }
        for (int i = index + 1; i < s.Length; i++) // get endIndex
        {
            if (s[i] is '+' or '-' or '(' or ')')
            {
                endIndex = i;
                break;
            }
        }

        return s[startIndex..endIndex];
    }
    private string GetStartOperationAt(string s, int index)
    {
        int startIndex = 0;
        int endIndex = s.Length;

        for (int i = index - 1; i >= 0; i--) // get startIndex
        {
            if (s[i] is '+' or '-' or '(' or ')')
            {
                startIndex = i + 1;
                break;
            }
        }
        for (int i = index + 1; i < s.Length; i++) // get endIndex
        {
            if (s[i] is '+' or '-' or '(' or ')')
            {
                endIndex = i - 1;
                break;
            }
        }

        return s[startIndex..endIndex];
    }
    private string GetBracketAt(string s, int index)
    {
        int startIndex = 0;
        int endIndex = s.Length;

        int layer = 0;

        for (int i = index; i >= 0; i--)
        {
            if (s[i] is '(')
            {
                startIndex = i;
                for (int j = i; j < s.Length; j++) // find out layer (I hope no one has to read this except for me)
                {
                    if (s[j] == '(') layer++;
                    else break;
                }
                break;
            }
        }

        for (int i = index; i < s.Length; i++)
        {
            if (s[i] is ')')
            {
                if (--layer == 0)
                {
                    endIndex = i + 1;
                    break;
                }
            }
        }

        return s[startIndex..endIndex];
    }

    private string EvaluateBracketSigns(string s)
    {
        s = s.Replace(" ", "");

        while (GetMaxBracketLayer(s) > 0)
        {
            int targetLayer = GetMaxBracketLayer(s);
            int currentLayer = 0;

            int closedIndex;
            int openIndex = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '(') //* find out open- and closed index
                {
                    if (++currentLayer == targetLayer) openIndex = i;
                    continue;
                }
                else if (s[i] == ')')
                {
                    if (--currentLayer == targetLayer - 1)
                    {
                        closedIndex = i;
                    }
                    else continue;
                }
                else continue;

                if (openIndex == targetLayer - 1 || s[openIndex - 1] == '+')
                {
                    s = s.Remove(openIndex, 1).Remove(closedIndex - 1, 1);
                }
                else if (s[openIndex - 1] is '-')
                {
                    StringBuilder b = new(s);
                    for (int j = openIndex + 1; j < closedIndex; j++)
                    {
                        if (b[j] == '+') b[j] = '-';
                        else if (b[j] == '-') b[j] = '+';
                    }
                    s = b.Remove(openIndex, 1).Remove(closedIndex - 1, 1).ToString(); // remove enclosing brackets
                }
            }
        }


        return s.Replace("-+", "+"); // fix weird case
    }

    private int GetMaxBracketLayer(string s)
    {
        int layer = 0;
        int maxLayer = 0;
        for (int i = 0; i < s.Length; i++)
        {
            if (s[i] == '(') layer++;
            else if (s[i] == ')') layer--;

            if (layer > maxLayer) maxLayer = layer;
        }
        return maxLayer;
    }

    private string SimplifyValueString(string s)
    {
        //* Find all variable combinations with their coefficients
        string[] products = s.Replace(" ", "").Replace("+", " +").Replace("-", " -").Split(' ');
        products[0] = products[0].Insert(0, "+");

        List<VariablePair> pairs = new();

        for (int i = 0; i < products.Length; i++)
        {
            bool negative = products[i][0] == '-';
            products[i] = products[i][1..]; // trim first sign

            if (products[i] == "") continue; // for the case that there's a '-' sign at the start

            //* Build variables and coefficients
            string[] parts = products[i].Replace("/", " /").Replace("*", " *").Split(' ');
            parts[0] = parts[0].Insert(0, "*");

            double coefficient = 1;
            string variable = "";
            for (int j = 0; j < parts.Length; j++)
            {
                if (allowedVariableNames.Contains(parts[j][1]))
                {
                    variable += parts[j];
                    continue;
                }

                char op = parts[j][0];
                double num = double.Parse(parts[j][1..]);

                coefficient = op switch
                {
                    '*' => coefficient * num,
                    '/' => coefficient / num,
                    _ => coefficient
                };
            }

            if (negative) coefficient = -coefficient;

            //* simplfify variables
            variable = SimplifyVariableProducts(variable);

            //* add to list
            pairs.Add(new(coefficient, variable));
        }


        //* Add all coefficients together
        string result = "";
        while (pairs.Count > 0)
        {
            VariablePair[] coefficientsForVar = pairs.Where(p => p.variable == pairs[0].variable).ToArray();

            double coefficient = 0;
            foreach (VariablePair c in coefficientsForVar) coefficient += c.coefficient;

            if (coefficient == 1 && pairs[0].variable != "") result += pairs[0].variable;
            else if (coefficient == -1 && pairs[0].variable != "") result += "-" + pairs[0].variable;
            else if (coefficient != 0) result += coefficient + pairs[0].variable;
            result += "+";

            pairs = pairs.Where(p => !p.variable.Equals(pairs[0].variable)).ToList();
        }

        return result.Replace("+-", "-")[..^1]; // replace "+-" with "-" and trim trailing '+'
    }

    private string SimplifyVariableProducts(string s)
    {
        //* Initialize
        s = s.Replace(" ", "").TrimStart('*');

        /*for (int i = s.Length - 1; i >= 0; i--) //! Convert Powers
        {
            if (s[i] != '^') continue;

            int index = i + 1;
            while (char.IsNumber(s[index]) || s[i] == ',')
            {
                index++;
                if (index >= s.Length) break;
            }
            double power = double.Parse(s[(i + 1)..index]);

            char variable = s[i - 1];

            s = s.Replace($"{variable}^{power}", MultiplyString("*" + variable, (int)power)[1..]);
        }*/

        string numerator = "";
        string denominator = "";

        string[] parts = s.Replace("*", " *").Replace("/", " /").Split(' ');

        parts[0] = parts[0].Insert(0, "*"); // first part always in numerator

        //* Build numerator and denominator
        foreach (string part in parts)
        {
            if (part[0] == '*') numerator += part[1..];
            else if (part[0] == '/') denominator += part[1..];
            else throw new ArgumentException($"Wrong operator in argument. Parameter name: {nameof(s)}");
        }

        //* Early return if both are the same
        if (numerator == denominator) return "";

        string result = "";

        //* Simplify numerator and denominator
        foreach (char variable in allowedVariableNames)
        {
            int numeratorCount = numerator.Count(c => c == variable);
            int denominatorCount = denominator.Count(c => c == variable);

            int power = numeratorCount - denominatorCount;

            if (power != 0)
            {
                result += variable;
                if (power != 1) result += "^" + power;
            }
        }

        return result;
    }
    private string MultiplyString(string s, int i)
    {
        string result = "";
        for (int j = 0; j < i; j++)
        {
            result += s;
        }
        return result;
    }
}