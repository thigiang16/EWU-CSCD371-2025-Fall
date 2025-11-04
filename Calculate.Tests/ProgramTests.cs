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
        string output = string.Empty;
        var program = new Program
        {
            WriteLine = s => output = s
        };
        string input = "Hello World!";
        
        program.WriteLine(input);
  
        Assert.AreEqual<string>(input, output);
    }

    [TestMethod]
    public void ReadLine_WorkAsExpected()
    {
        string input = "Hello";
        var program = new Program
        {
            ReadLine = () => input
        };

        string? receivedInput = program.ReadLine();

        Assert.AreEqual<string>(input, receivedInput);
    }
}
