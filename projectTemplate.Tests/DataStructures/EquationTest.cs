using JetBrains.Annotations;
using MathTools.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace projectTemplate.Tests.DataStructures;

[TestClass]
[TestSubject(typeof(Equation))]
public class EquationTest {

    [TestMethod]
    public void TestEquation_Quadratic() {
        var expected = "c=3";
        var actual = new Equation("c^4/(c*c)=3(1+2)").Solve();

        Assert.AreEqual(expected, actual);
    }
    
    [TestMethod]
    public void TestEquation_InfinteSolutions() {
        var expected = Master.infiniteSolutionsText;
        var actual = new Equation("x^2-(34x/x)=x^4/(x*x)+(56-90)").Solve();

        Assert.AreEqual(expected, actual);
    }
}