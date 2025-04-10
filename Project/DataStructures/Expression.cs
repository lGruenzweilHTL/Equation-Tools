using System.Text;
using System.Text.RegularExpressions;
using static Master;
using MathTools.Internal;

namespace MathTools;

public class Expression {
    public Expression(string s) {
        Value = s;
    }

    public string Value { get; private set; }
    public int VariableCount => Value.Count(c => AllowedVariableNames.Contains(c));

    public string Simplify() {
        return Simplify(Value);
    }

    private string Simplify(string s) {
        if (string.IsNullOrEmpty(s)) return "";

        s = s.Format();

        if (VariableCount == 0) return new Calculation(s).Evaluate();

        s = EvaluateBracketCoefficients(s);
        s = EvaluateBracketSigns(s);
        s = SimplifyValueString(s);

        return s.TrimStart('+');
    }

    private string EvaluateBracketCoefficients(string s) {
        s = s.Replace(" ", "");

        // Early return if there aren't any brackets to evaluate
        if (!s.Contains('(')) return s;

        int targetLayer = GetMaxBracketLayer(s);
        while (targetLayer > 0) {
            int currentLayer = 0;

            int closedIndex;
            int openIndex = 0;


            for (int i = 0; i < s.Length; i++) {
                bool bracketTimesBracket = false;

                if (s[i] == '(') //* find out open- and closedIndex
                {
                    if (++currentLayer == targetLayer) openIndex = i;
                    continue;
                }
                else if (s[i] == ')') {
                    if (--currentLayer == targetLayer - 1) {
                        closedIndex = i;
                    }
                    else continue;
                }
                else continue;

                if (s[openIndex] != '(') break; // fix weird case with nested brackets

                bool dotOperationAtStart = false;
                if (openIndex != 0) {
                    dotOperationAtStart = s[openIndex - 1] == '*' || s[openIndex - 1] == '/';
                }

                bool dotOperationAtEnd = false;
                if (closedIndex != s.Length - 1) {
                    dotOperationAtEnd = s[closedIndex + 1] == '*' || s[closedIndex + 1] == '/';
                }

                if (!dotOperationAtStart && !dotOperationAtEnd) {
                    targetLayer--;
                    break;
                }

                string coefficient = "";

                string startProduct = "";
                string endProduct = "";
                bool divisionByBracket = false;

                if (openIndex > 1 && s[openIndex - 1] is '*' or '/') {
                    if (s[openIndex - 2] == ')') {
                        startProduct = s.GetBracketAtIndex(openIndex - 2);
                        bracketTimesBracket = true;
                    }
                    else startProduct = GetStartOperationAt(s, openIndex - 2);
                }

                if (closedIndex < s.Length - 2 && s[closedIndex + 1] is '*' or '/') {
                    if (s[closedIndex + 2] == '(') {
                        endProduct = s.GetBracketAtIndex(closedIndex + 2);
                        bracketTimesBracket = true;
                    }
                    else endProduct = GetEndOperationAt(s, closedIndex + 2);
                }

                if (startProduct != "") {
                    if (dotOperationAtStart && s[openIndex - 1] == '*') {
                        coefficient += "*" + startProduct;
                    }

                    if (dotOperationAtStart && s[openIndex - 1] == '/') {
                        StringBuilder bracket = new(s.GetBracketAtIndex(closedIndex));
                        for (int j = 0; j < bracket.Length; j++) {
                            if (bracket[j] == '*') bracket[j] = '/';
                            else if (bracket[j] == '/') bracket[j] = '*';

                            if (bracket[j] == '^') {
                                if (bracket[j + 1] == '-') bracket[j + 1] = '+';
                                else bracket = bracket.Insert(j, '-');
                            }
                        }


                        s = s.Remove(openIndex, closedIndex + 1 - openIndex); // Remove original bracket
                        s += bracket.ToString()[1..^1]; // Add inverted bracket

                        divisionByBracket = true;
                    }
                }

                if (endProduct != "") {
                    if (dotOperationAtEnd && s[closedIndex + 1] == '*') {
                        coefficient += "*" + endProduct;
                    }

                    if (dotOperationAtEnd && s[closedIndex + 1] == '/') {
                        coefficient += "/" + endProduct;
                    }
                }

                if (!divisionByBracket) {
                    // Moved from inside the loop to outside (only at the end),
                    // because this led to some bugs with nested brackets
                    // Example of error: "3*((3*x))" would simplify to "27x"
                    s = s.Insert(closedIndex, coefficient);
                    for (int j = closedIndex - 1; j >= openIndex; j--) {
                        if (s[j] is '+' or '-') {
                            s = s.Insert(j, coefficient);
                        }
                    }
                }

                // Remove the start product
                if (startProduct != "" && !divisionByBracket)
                    s = s.Remove(openIndex - startProduct.Length - 1, startProduct.Length + 1);

                // Find new closedIndex (because of insertions and deletions)
                closedIndex = FindNewClosedIndex(s, targetLayer, 0);
                if (closedIndex == -1) return s;

                // Remove the end product
                if (endProduct != "") s = s.Remove(closedIndex + 1, endProduct.Length + 1);

                // if this was the last operation on this layer, decrement to work on the next one
                bool lastOpOnLayer = s[closedIndex..].Count(c => c == ')') == targetLayer;
                if (!bracketTimesBracket) {
                    if (lastOpOnLayer) targetLayer--;
                }
                else targetLayer++;

                i = closedIndex + 1;
            }
        }

        return s;
    }

    private int FindNewClosedIndex(string s, int targetLayer, int startIndex) {
        int layer = 0;
        for (int j = startIndex; j < s.Length; j++) {
            if (s[j] == '(') layer++;
            if (s[j] == ')') {
                if (layer == targetLayer) {
                    return j;
                }

                layer--;
            }
        }

        return -1;
    }

    private string GetEndOperationAt(string s, int index) {
        int startIndex = 0;
        int endIndex = s.Length;

        for (int i = index - 1; i >= 0; i--) // get startIndex
        {
            if (s[i] is '+' or '-' or '(' or ')') {
                startIndex = i + 2;
                break;
            }
        }

        for (int i = index + 1; i < s.Length; i++) // get endIndex
        {
            if (s[i] is '+' or '-' or '(' or ')') {
                endIndex = i;
                break;
            }
        }

        return s[startIndex..endIndex];
    }

    private string GetStartOperationAt(string s, int index) {
        int startIndex = 0;
        int endIndex = s.Length;

        for (int i = index - 1; i >= 0; i--) // get startIndex
        {
            if (s[i] is '+' or '-' or '(' or ')') {
                startIndex = i + 1;
                break;
            }
        }

        for (int i = index + 1; i < s.Length; i++) // get endIndex
        {
            if (s[i] is '+' or '-' or '(' or ')') {
                endIndex = i - 1;
                break;
            }
        }

        return s[startIndex..endIndex];
    }

    private string EvaluateBracketSigns(string s) {
        s = s.Replace(" ", "");

        while (GetMaxBracketLayer(s) > 0) {
            int targetLayer = GetMaxBracketLayer(s);
            int currentLayer = 0;

            int closedIndex;
            int openIndex = 0;
            for (int i = 0; i < s.Length; i++) {
                if (s[i] == '(') //* find out open- and closed index
                {
                    if (++currentLayer == targetLayer) openIndex = i;
                    continue;
                }
                else if (s[i] == ')') {
                    if (--currentLayer == targetLayer - 1) {
                        closedIndex = i;
                    }
                    else continue;
                }
                else continue;

                if (openIndex == targetLayer - 1 || s[openIndex - 1] == '+') {
                    s = s.Remove(openIndex, 1).Remove(closedIndex - 1, 1);
                }
                else if (s[openIndex - 1] is '-') {
                    StringBuilder b = new(s);
                    for (int j = openIndex + 1; j < closedIndex; j++) {
                        if (b[j] == '+') b[j] = '-';
                        else if (b[j] == '-') b[j] = '+';
                    }

                    s = b.Remove(openIndex, 1).Remove(closedIndex - 1, 1).ToString(); // remove enclosing brackets
                }
                else return s;
            }
        }


        return s.Replace("-+", "+"); // fix weird case
    }

    private int GetMaxBracketLayer(string s) {
        int layer = 0;
        int maxLayer = 0;
        for (int i = 0; i < s.Length; i++) {
            if (s[i] == '(') layer++;
            else if (s[i] == ')') layer--;

            if (layer > maxLayer) maxLayer = layer;
        }

        return maxLayer;
    }

    private string SimplifyValueString(string s) {
        //* Find all variable combinations with their coefficients
        string[] products = s.Replace(" ", "").Replace("+", " +").Replace("-", " -").Split(' ');
        products[0] = products[0].Insert(0, "+");

        List<VariablePair> pairs = new();

        for (int i = 0; i < products.Length; i++) {
            bool negative = products[i][0] == '-';
            products[i] = products[i][1..]; // trim first sign

            if (products[i] == "") continue; // for the case that there's a '-' sign at the start

            //* Build variables and coefficients
            string[] parts = products[i].Replace("/", " /").Replace("*", " *").Split(' ');
            parts[0] = parts[0].Insert(0, "*");

            double coefficient = 1;
            string variable = "";
            for (int j = 0; j < parts.Length; j++) {
                if (AllowedVariableNames.Contains(parts[j][1])) {
                    variable += parts[j];
                    continue;
                }

                char op = parts[j][0];
                double num = double.Parse(parts[j][1..]);

                coefficient = op switch {
                    '*' => coefficient * num,
                    '/' => coefficient / num,
                    _ => coefficient
                };
            }

            if (negative) coefficient = -coefficient;

            //* simplify variables
            variable = SimplifyVariableProducts(variable);

            //* add to list
            pairs.Add(new(coefficient, variable));
        }


        //* Add all coefficients together
        string result = "";
        while (pairs.Count > 0) {
            VariablePair[] coefficientsForVar = pairs.Where(p => p.Variable == pairs[0].Variable).ToArray();

            double coefficient = 0;
            foreach (VariablePair c in coefficientsForVar) coefficient += c.Coefficient;

            if (coefficient == 1 && pairs[0].Variable != "") result += pairs[0].Variable;
            else if (coefficient == -1 && pairs[0].Variable != "") result += "-" + pairs[0].Variable;
            else if (coefficient != 0) result += coefficient + pairs[0].Variable;
            result += "+";

            pairs = pairs.Where(p => !p.Variable.Equals(pairs[0].Variable)).ToList();
        }

        return result.Replace("+-", "-")[..^1]; // replace "+-" with "-" and trim trailing '+'
    }

    private string SimplifyVariableProducts(string s) {
        //* Initialize
        s = s.Replace(" ", "").TrimStart('*');

        string numerator = "";
        string denominator = "";

        string[] parts = s.Replace("*", " *").Replace("/", " /").Split(' ');

        parts[0] = parts[0].Insert(0, "*"); // first part always in numerator

        //* Build numerator and denominator
        foreach (string part in parts) {
            if (part[0] == '*') numerator += part[1..];
            else if (part[0] == '/') denominator += part[1..];
            else throw new ArgumentException($"Wrong operator in argument. Parameter name: {nameof(s)}");
        }

        //* Early return if both are the same
        if (numerator == denominator) return "";

        string result = "";

        //* Simplify numerator and denominator
        foreach (char variable in AllowedVariableNames) {
            double power = 0;

            //* Find power of variable in numerator
            for (int i = 0; i < numerator.Length; i++) {
                if (numerator[i] == variable) {
                    double individualPower = 1;
                    if (i < numerator.Length - 1 && numerator[i + 1] == '^') // if power sign exists
                    {
                        // find power
                        int numberStart = i + 2;
                        int numberEnd = numberStart;
                        while (numberEnd < numerator.Length &&
                               (char.IsNumber(numerator[numberEnd]) || numerator[numberEnd] == ',')) numberEnd++;
                        individualPower = double.Parse(numerator[numberStart..numberEnd]);
                    }

                    power += individualPower;
                }
            }

            for (int i = 0; i < denominator.Length; i++) {
                if (denominator[i] == variable) {
                    double individualPower = 1;
                    if (i < denominator.Length - 1 && denominator[i + 1] == '^') // if power sign exists
                    {
                        // find power
                        int numberStart = i + 2;
                        int numberEnd = numberStart;
                        while (numberEnd < denominator.Length &&
                               (char.IsNumber(denominator[numberEnd]) || denominator[numberEnd] == ',')) numberEnd++;
                        individualPower = double.Parse(denominator[numberStart..numberEnd]);
                    }

                    power -= individualPower;
                }
            }


            if (power != 0) {
                result += variable;
                if (power != 1) result += "^" + power;
            }
        }

        return result;
    }
}