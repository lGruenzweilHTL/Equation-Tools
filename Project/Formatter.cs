using static Master;

namespace MathTools.Internal;

public static class Formatter
{
    public static string Format(this string s)
    {
        s = s.Replace(" ", "");
        s = s.FillMissingEndBrackets();
        s = s.ReplaceEAndPi();
        s = s.HandleVariableCoefficients();
        s = s.HandleBracketCoefficients();

        return s;
    }


    public static string FillMissingEndBrackets(this string s)
    {
        int open = s.Count(c => c == '(');
        int closed = s.Count(c => c == ')');

        if (open > closed) s += new string(')', open - closed);

        return s;
    }
    public static string ReplaceEAndPi(this string s) => s.Replace("e", $"({Math.E})").Replace("pi", $"({Math.PI})");
    public static string HandleVariableCoefficients(this string s)
    {
        for (int i = s.Length - 1; i > 0; i--)
        {
            bool previousIsCoefficient = char.IsNumber(s[i - 1]) || AllowedVariableNames.Contains(s[i - 1]);
            if (AllowedVariableNames.Contains(s[i]) && previousIsCoefficient) s = s.Insert(i, "*");
        }

        return s;
    }
    public static string HandleBracketCoefficients(this string s)
    {
        for (int i = s.Length - 1; i >= 0; i--)
        {
            if (i > 0)
            {
                // only have to check for ')' once, because it doesn't matter if it's handled at the start of the previous or at the end of the next bracket
                bool previousIsCoefficient = char.IsNumber(s[i - 1]) || AllowedVariableNames.Contains(s[i - 1]) || s[i - 1] == ')';
                if (s[i] == '(' && previousIsCoefficient) s = s.Insert(i, "*");
            }
            if (i < s.Length - 1)
            {
                // check for ',' incase decimal numbers are written like ",4" instead of "0,4"
                bool nextIsCoefficient = char.IsNumber(s[i + 1]) || AllowedVariableNames.Contains(s[i + 1]) || s[i + 1] == ',';
                if (s[i] == ')' && nextIsCoefficient) s = s.Insert(i + 1, "*");
            }
        }

        return s;
    }
}