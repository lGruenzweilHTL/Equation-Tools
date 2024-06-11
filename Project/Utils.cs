namespace MathTools.Internal;

public static class Utils {
    public static double GetParsedDecimalNumberAtIndex(this string s, int index) =>
        double.Parse(s.GetDecimalNumberAtIndex(index));

    public static string GetDecimalNumberAtIndex(this string s, int index) {
        if (!s[index].IsPartOfDecimalNumber()) return "";

        int startIndex = index;
        while (startIndex >= 0 && s[startIndex].IsPartOfDecimalNumber()) startIndex--;
        startIndex++;

        int endIndex = index + 1;
        while (endIndex < s.Length && s[endIndex].IsPartOfDecimalNumber()) endIndex++;

        return s[startIndex..endIndex];
    }

    public static bool IsPartOfDecimalNumber(this char c) => char.IsNumber(c) || c == ',';
    
    public static string GetBracketAtIndex(this string s, int index) {
        return s.GetBracketAtIndex(index, out int _, out int _);
    }

    public static string GetBracketAtIndex(this string s, int index, out int startIndex, out int endIndex) {
        startIndex = 0;
        endIndex = s.Length;

        int layer = 0;

        for (int i = index; i >= 0; i--) {
            if (s[i] == '(') {
                startIndex = i;
                for (int j = i; j < s.Length; j++) // find out layer (I hope no one has to read this except for me)
                {
                    if (s[j] == '(') layer++;
                    else break;
                }

                break;
            }
        }

        for (int i = index; i < s.Length; i++) {
            if (s[i] is ')') {
                if (--layer == 0) {
                    endIndex = i + 1;
                    break;
                }
            }
        }

        return s[startIndex..endIndex];
    }

    public static string Repeat(this string s, uint numberOfTimes) {
        string result = "";
        for (int i = 0; i < numberOfTimes; i++) result += s;
        return result;
    }
}