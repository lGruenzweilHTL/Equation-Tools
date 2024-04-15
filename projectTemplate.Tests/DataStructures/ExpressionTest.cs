using System;
using JetBrains.Annotations;
using MathTools;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace projectTemplate.Tests.DataStructures;

[TestClass]
[TestSubject(typeof(Expression))]
public class ExpressionTest {
    [TestMethod]
    public void TestExpression_Powers_Formatting() {
        var expected = "x^3-4x";
        var actual = new Expression("x(x^2 - 2(2) )").Simplify();

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestExpression_MultiBrackets() {
        var expected = "3x^3-8x^2-20x+16";
        var actual = new Expression("(x+2)(x-4)(3x-2)").Simplify();

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestExpression_PiAndE() {
        var expected = Math.Pow(double.Pi, double.E).ToString();
        var actual = new Expression("pi^e").Simplify();

        Assert.AreEqual(expected, actual);
    }
}