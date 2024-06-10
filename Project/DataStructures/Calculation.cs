namespace MathTools.Internal;

public class Calculation
{
    public Calculation(string s)
    {
        current = s;
    }
    private string current;
    public bool IsValid()
    {
        // if there is no occurence of any allowed variable, return true
        return current.IndexOfAny(Master.AllowedVariableNames) == -1;
    }

    public string Evaluate()
    {
        try
        {
            return Evaluate(current);
        }
        catch { throw; }
    }
    public bool TryEvaluate(out string result)
    {
        result = "";
        if (!IsValid()) return false;

        try
        {
            result = Evaluate();
            return true;
        }
        catch { return false; }
    }
    private string Evaluate(string s)
    {
        try
        {
            s = s.Replace(" ", "");
            s = EvaluateBrackets(s).Replace(" ", "");
            s = EvaluatePowers(s).Replace(" ", "");
            s = EvaluateDotOperations(s).Replace(" ", "");
            s = EvaluateLineOperations(s).ToString();

            return s;
        }
        catch { throw; } // rethrow to be handled in other method
    }
    private string EvaluateBrackets(string s)
    {
        if (!s.Contains('(')) return s;

        int endIndex = 0;
        int currentLayer = 0;

        for (int i = s.Length - 1; i >= 0; i--)
        {
            EvaluateLayers(i);
        }

        return s;

        void EvaluateLayers(int i)
        {
            if (s[i] == ')')
            {
                currentLayer++;
                if (currentLayer == 1) endIndex = i;
            }
            else if (s[i] == '(')
            {
                currentLayer--;
                if (currentLayer != 0) return;

                string stringToEvaluate = s[i..(endIndex + 1)]; // for example "(2+3)"
                string eval = Evaluate(stringToEvaluate[1..^1]); // evaluates only "2+3"

                s = s.Replace(stringToEvaluate, eval); // replaces "(2+3)" with 5
            }
        }
    }
    private string EvaluatePowers(string s)
    {
        if (!s.Contains('^')) return s;

        string[] powerOperations = s
            .Replace("+", " + ")
            .Replace("-", " - ")
            .Replace("*", " * ")
            .Replace("/", " / ")
            .Split(' ')
        ;

        FixNegatives(powerOperations);

        for (int i = 0; i < powerOperations.Length; i++)
        {
            if (powerOperations[i].Contains('^'))
            {
                powerOperations[i] = powerOperations[i].Replace("^", " ^");
                string[] parts = powerOperations[i].Split(' ');

                double result = double.Parse(parts[0]);
                for (int j = 1; j < parts.Length; j++)
                {
                    double operand = double.Parse(parts[j][1..]);

                    result = parts[j][0] switch
                    {
                        '^' => Math.Pow(result, operand),
                        _ => result
                    };
                }

                powerOperations[i] = result.ToString();
            }
        }
        return string.Join(' ', powerOperations);
    }
    private string EvaluateDotOperations(string s)
    {
        if (!s.Contains('*') && !s.Contains('/')) return s;

        string[] dotOperations = s.Replace("+", " + ").Replace("-", " - ").Split(' ');

        FixNegatives(dotOperations);

        for (int i = 0; i < dotOperations.Length; i++)
        {
            if (dotOperations[i].Contains('*') || dotOperations[i].Contains('/'))
            {
                dotOperations[i] = dotOperations[i].Replace("*", " *").Replace("/", " /");
                string[] parts = dotOperations[i].Split(' ');

                double result = double.Parse(parts[0]);
                for (int j = 1; j < parts.Length; j++)
                {
                    double operand = double.Parse(parts[j][1..]);

                    result = parts[j][0] switch
                    {
                        '*' => result * operand,
                        '/' => result / operand,
                        _ => result
                    };
                }

                dotOperations[i] = result.ToString();
            }
        }
        return string.Join(' ', dotOperations);
    }
    private static double EvaluateLineOperations(string s)
    {
        double result = 0;

        // handle weird case with negative numbers
        s = s.Replace("--", "+");


        // 15 - 10 is converted to 15 +- 10
        // so the array will be {15, -10}
        // and the calculation will be correct
        s = s.Replace("-", "+-");

        foreach (string item in s.Replace(" ", "").Split('+'))
        {
            if (item.ToString() != "")
            {
                result += double.Parse(item);
            }
        }
        return result;
    }
    private static void FixNegatives(string[] operations)
    {
        if (operations.Length == 0) return;

        // if the string is for example "8*-4" it will get split to {"8*", "-", "4"}
        // which will result in an error because "8*" is not parsable to a double
        // so we will reset the first operation to 0 (same as remove it)
        // and add it to the start of the third (the second is just "-" so it will get ignored)
        // this will result in {"0", "-", "8*4"} which will hopefully work
        if (operations[0].EndsWith('*') || operations[0].EndsWith('/') || operations[0].EndsWith('^'))
        {
            string entire = operations[0];
            operations[0] = "0";

            // add entire first dotOperation to the start of dotOperations[2]
            // everytime this case happens, there will be at least 3 dotOperations
            // so it will never give an IndexOutOfRangeException
            operations[2] = entire + operations[2];
        }

        if (operations.Length < 3) return;

        if (operations[2].EndsWith('*') || operations[2].EndsWith('/') || operations[2].EndsWith('^'))
        {
            operations[0] = "";
            operations[1] = "";
            operations[3] = "";
            string lastOp = operations[4];
            operations[4] = "";

            operations[2] += lastOp;
        }
    }
}
