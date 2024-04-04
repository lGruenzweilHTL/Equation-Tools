/*
    *Info:

    Calculations are collections of numbers and operators without variables
    Expressions are Calculations but with variables (can only be simplified, not calculated)
    Equations are a combination of a left expression, an '=' sign and a right expression (only a single variable)


    *Features:

    Basic powers
    E and Pi
    Short forms like "2x"
    Short forms like "2(1+3)"
    Exponents in equation (like "x^2 = 25") (gives only one answer)


    *Errors:

    Any Expression/Equation with a variable in the exponent
*/

public static class Master
{
    public static char[] allowedVariableNames = "xyzabcd".ToCharArray();

    public record VariablePair(double coefficient, string variable);

    public static string infiniteSolutionsText = "infinite solutions";
    public static string noSolutionsText = "no solution";
}