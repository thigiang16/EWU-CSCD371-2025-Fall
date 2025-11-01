using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculate;
using System.Collections.Generic;

namespace Calculate.Tests;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void WriteLine_WorkAsExpected()
    {
        List<string> output = new();
        Program io = new Program
        {
            WriteLine = (s) => output.Add(s)
        };
        io.WriteLine("Hello World!");

        Assert.HasCount(1, output);
        Assert.AreEqual<string>("Hello World!", output[0]);
    }

    [TestMethod]
    public void ReadLine_WorkAsExpected()
    {
        Queue<string> input = new(new[] { "Hi" });
        Program io = new Program
        {
            ReadLine = () => input.Dequeue()
        };
        string? result = io.ReadLine();
        Assert.AreEqual<string>("Hi", result);
    }
}
