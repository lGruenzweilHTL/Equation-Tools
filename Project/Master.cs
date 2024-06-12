public static class Master
{
    public static char[] AllowedVariableNames = "xyzabcd".ToCharArray();

    public record VariablePair(double Coefficient, string Variable);

    public static string InfiniteSolutionsText = "infinite solutions";
    public static string NoSolutionsText = "no solution";
}