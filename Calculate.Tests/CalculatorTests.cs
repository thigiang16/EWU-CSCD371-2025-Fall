namespace Calculate.Tests;

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    public void TryCalculate_Addition_ReturnsExpectedResult()
    {
        bool ok = Calculator.TryCalculate("3 + 4", out double result);
        Assert.IsTrue(ok);
        Assert.AreEqual<double>(7, result);
    }

    [TestMethod]
    public void TryCalculate_Subtraction_ReturnsExpectedResult()
    {
        bool ok = Calculator.TryCalculate("10 - 7", out double result);
        Assert.IsTrue(ok);
        Assert.AreEqual<double>(3, result);
    }

    [TestMethod]
    public void TryCalculate_Multiplication_ReturnsExpectedResult()
    {
        bool ok = Calculator.TryCalculate("2 * 7", out double result);
        Assert.IsTrue(ok);
        Assert.AreEqual<double>(14, result);
    }

    [TestMethod]
    public void TryCalculate_Division_ReturnsExpectedResult()
    {
        bool ok = Calculator.TryCalculate("10 / 5", out double result);
        Assert.IsTrue(ok);
        Assert.AreEqual<double>(2, result);
    }

    [TestMethod]
    public void TryCalculate_NoSpacesAroundOperator_ReturnsFalse()
    {
        bool temp = Calculator.TryCalculate("3+4", out double result);
        Assert.IsFalse(temp);
    }

    [TestMethod]
    public void TryCalculate_NonIntegerInput_ReturnsFalse()
    {
        bool temp = Calculator.TryCalculate("3.5 + 2", out double result);
        Assert.IsFalse(temp);
    }

    [TestMethod]
    public void TryCalculate_DivideByZero_ReturnsFalse()
    {
        bool temp = Calculator.TryCalculate("10 / 0", out double result);
        Assert.IsFalse(temp);
    }

    
}
