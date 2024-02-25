/*
    *Info:

    Calculations are collections of numbers and operators without variables
    Expressions are Calculations but with variables (can only be simplified, not calculated)
    Equations are a combination of a left expression, an '=' sign and a right expression (only a single variable)
    Formulas are Equations that allow multiple variables

    TODO: make all these types
*/

public static class Master
{
    public static char[] allowedVariableNames = "xyz".ToCharArray();

    public record VariablePair(double coefficient, string variable);
}