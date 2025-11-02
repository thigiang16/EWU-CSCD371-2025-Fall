namespace Calculate.Tests;

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    public void TryCalculate_Addition_ReturnsExpectedResult()
    {
        Calculator calc = new();
        bool ok = calc.TryCalculate("3 + 4", out double result);
        Assert.IsTrue(ok);
        Assert.AreEqual<double>(7, result);
    }

    [TestMethod]
    public void TryCalculate_NoSpacesAroundOperator_ReturnsFalse()
    {
        Calculator calc = new();
        bool temp = calc.TryCalculate("3+4", out double result);
        Assert.IsFalse(temp);
    }

    [TestMethod]
    public void TryCalculate_NonIntegerInput_ReturnsFalse()
    {
        Calculator calc = new();
        bool temp = calc.TryCalculate("3.5 + 2", out double result);
        Assert.IsFalse(temp);
    }

    [TestMethod]
    public void TryCalculate_DivideByZero_ReturnsFalse()
    {
        Calculator calc = new();
        bool temp = calc.TryCalculate("10 / 0", out double result);
        Assert.IsFalse(temp);
    }
}
