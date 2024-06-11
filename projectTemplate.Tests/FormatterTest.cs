using static System.Math;
using FluentAssertions;
using JetBrains.Annotations;
using MathTools.Internal;
using Xunit;

namespace projectTemplate.Tests;

[TestSubject(typeof(Formatter))]
public class FormatterTest {

    [Theory]
    [InlineData("", "")]
    [InlineData("(x)", "(x)")]
    [InlineData("1+2-(3)", "1+2-(3)")]
    [InlineData("1-2-(5x", "1-2-(5x)")]
    [InlineData("1+(1+(1+(1+(1+(1", "1+(1+(1+(1+(1+(1)))))")]
    [InlineData("1+(2)+(((2+5)+2", "1+(2)+(((2+5)+2))")]
    public void TestFormatting_MissingEndBrackets(string expr, string expected) {
        expr.FillMissingEndBrackets().Should().NotBeNull().And.Be(expected);
    }
    
    [Fact]
    public void TestFormatting_EPi() {
        string expr = "epiepiepie";
        string e = $"({E})";
        string pi = $"({PI})";

        string expected = e + pi + e + pi + e + pi + e;
            
        expr.ReplaceEAndPi().Should().NotBeNull().And.Be(expected);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData("x+y+z", "x+y+z")]
    [InlineData("7x-5y+z", "7*x-5*y+z")]
    [InlineData("7*5x", "7*5*x")]
    public void TestFormatting_VariableCoefficients(string expr, string expected) {
        expr.HandleVariableCoefficients().Should().NotBeNull().And.Be(expected);
    }
    
    [Theory]
    [InlineData("", "")]
    [InlineData("x+y+(z)", "x+y+(z)")]
    [InlineData("7(x-5*y)+z", "7*(x-5*y)+z")]
    [InlineData("7*x(2+3)", "7*x*(2+3)")]
    [InlineData("(x+1)(x+1)", "(x+1)*(x+1)")]
    public void TestFormatting_BracketCoefficients(string expr, string expected) {
        expr.HandleBracketCoefficients().Should().NotBeNull().And.Be(expected);
    }
    
    [Theory]
    [InlineData("", "")]
    [InlineData("(x+1)", "(x+1)")]
    [InlineData("(x-x)^2", "(x-x)(x-x)")]
    [InlineData("(x)^3", "(x)(x)(x)")]
    public void TestFormatting_BracketExponents(string expr, string expected) {
        expr.HandleBracketExponents().Should().NotBeNull().And.Be(expected);
    }
}