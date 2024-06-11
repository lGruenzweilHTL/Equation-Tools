using System;
using FluentAssertions;
using JetBrains.Annotations;
using MathTools.Internal;
using Xunit;

namespace projectTemplate.Tests.DataStructures;

[TestSubject(typeof(Equation))]
public class EquationTest {

    [Theory]
    [InlineData("x=3", "x=3")]
    [InlineData("2x+2=0", "x=-1")]
    [InlineData("a^3=125", "a=5")]
    [InlineData("(c+2)^2=25", "c=3")]
    [InlineData("z+z+z+2z+5z=10", "z=1")]
    [InlineData("d^2=144", "d=12")]
    public void TestEquation(string expr, string expected) {
        new Equation(expr).Solve().Should().NotBeNull().And.Be(expected);
    }

    [Fact]
    public void TestEquationThrows() {
        Assert.Throws<ArgumentException>(() => new Equation(""));
    }
}