using System;
using FluentAssertions;
using JetBrains.Annotations;
using MathTools;
using Xunit;

namespace projectTemplate.Tests.DataStructures;

[TestSubject(typeof(Expression))]
public class ExpressionTest {

    [Theory]
    [InlineData("", "")]
    [InlineData("3", "3")]
    [InlineData("1+2", "3")]
    [InlineData("5^3", "125")]
    [InlineData("x*y/z", "xyz^-1")]
    [InlineData("x*x/(x*x", "1")]
    [InlineData("pi^e", "22,45915771836104")]
    [InlineData("99*(99*99)*99+99^2-99/4*pi-e/2^7", "96069324,22434524")]
    [InlineData("(a+b)(a+c)", "a^2+ac+ab+bc")]
    [InlineData("1-(1-(1-(1-(1-(1-(1-(1-(1)", "1")]
    public void TestExpression(string expr, string expected) {
        new Expression(expr).Simplify().Should().NotBeNull().And.Be(expected);
    }
}