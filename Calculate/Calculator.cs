using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculate;

public class Calculator
{
    public static double Add(double a, double b) => a + b;
    public static double Subtract(double a, double b) => a - b;
    public static double Multiple(double  a, double b ) => a * b;
    public static double Divide(double a, double b)
    {
        if (b == 0) throw new DivideByZeroException("Cannot divide by zero");
        return a / b;
    }

    public IReadOnlyDictionary<char, Func<double, double, double>> MathematicalOperations { get; }
        = new Dictionary<char, Func<double, double, double>>
        {
            {'+', Add },
            {'-', Subtract },
            {'*', Multiple },
            {'/', Divide }
        };

    public bool TryCalculate(string calculation, out double result)
    {
        result = 0.0;

        if (string.IsNullOrWhiteSpace(calculation))
        {
            return false;
        }

        string[] parts = calculation.Split(' ');
        if (parts.Length != 3)
        {
            return false;
        }

        string leftTemp = parts[0];
        string opTemp = parts[1];
        string rightTemp = parts[2];

        if (opTemp.Length != 1)
        {
            return false;
        }

        if (!int.TryParse(leftTemp, out int left) || !int.TryParse(rightTemp, out int right))
        {
            return false;
        }

        char op = opTemp[0];

        if (!MathematicalOperations.TryGetValue(op, out var operation))
        {
            return false;
        }

        if (op == '/' && right == 0)
        {
            return false;
        }

        result = operation(left, right);
        return true;
    }
}
