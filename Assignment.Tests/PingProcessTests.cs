using IntelliTect.TestTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment.Tests;

[TestClass]
public class PingProcessTests
{
    PingProcess Sut { get; set; } = new();

    [TestInitialize]
    public void TestInitialize()
    {
        Sut = new();
    }

    [TestMethod]
    public void Start_PingProcess_Success()
    {
        Process process = Process.Start("ping", "localhost");
        process.WaitForExit();
        Assert.AreEqual<int>(0, process.ExitCode);
    }

    [TestMethod]
    public void Run_GoogleDotCom_Success()
    {
        int exitCode = Sut.Run("google.com").ExitCode;
        Assert.AreEqual<int>(0, exitCode);
    }


    [TestMethod]
    public void Run_InvalidAddressOutput_Success()
    {
        (int exitCode, string? stdOutput) = Sut.Run("badaddress");
        Assert.IsFalse(string.IsNullOrWhiteSpace(stdOutput));
        stdOutput = WildcardPattern.NormalizeLineEndings(stdOutput!.Trim());
        Assert.AreEqual<string?>(
            "Ping request could not find host badaddress. Please check the name and try again.".Trim(),
            stdOutput,
            $"Output is unexpected: {stdOutput}");
        Assert.AreEqual<int>(1, exitCode);
    }

    [TestMethod]
    public void Run_CaptureStdOutput_Success()
    {
        PingResult result = Sut.Run("localhost");
        AssertValidPingOutput(result);
    }

    [TestMethod]
    public void RunTaskAsync_Success()
    {
        // Do NOT use async/await in this test.
        // Test Sut.RunTaskAsync("localhost").

        Task<PingResult> task = Sut.RunTaskAsync("localhost");
        PingResult result = task.Result;

        AssertValidPingOutput(result);
        //Assert.AreEqual<int>(0, result.ExitCode);
        //Assert.IsTrue(result.StdOutput is not null && result.StdOutput.Length > 0);
    }

    [TestMethod]
    public void RunAsync_UsingTaskReturn_Success()
    {
        // Do NOT use async/await in this test.
        PingResult result = Sut.RunAsync("localhost").Result;
        // Test Sut.RunAsync("localhost");
        AssertValidPingOutput(result);
    }

    [TestMethod]
    async public Task RunAsync_UsingTpl_Success()
    {
        // DO use async/await in this test.
        PingResult result = await Sut.RunAsync("localhost");

        // Test Sut.RunAsync("localhost");
        AssertValidPingOutput(result);
    }


    [TestMethod]
    //[ExpectedException(typeof(AggregateException))]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrapping()
    {
        using CancellationTokenSource cts = new();
        cts.Cancel();

        Assert.Throws<AggregateException>(() =>
        {
            Sut.RunAsync("localhost", cts.Token).Wait();
        });

    }

    [TestMethod]
    //[ExpectedException(typeof(TaskCanceledException))]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrappingTaskCanceledException()
    {   
        using CancellationTokenSource cts = new();
        cts.Cancel();

        try
        {
            Sut.RunAsync("localhost", cts.Token).Wait();
        }
        catch (AggregateException ex)
        {
            Exception? inner = ex.Flatten().InnerException;
            Assert.IsInstanceOfType(inner, typeof(TaskCanceledException));
        // Use exception.Flatten()
        }
    }

    [TestMethod]
    async public Task RunAsync_MultipleHostAddresses_True()
    {
        // Pseudo Code - don't trust it!!!
        string[] hostNames = new string[] { "localhost", "localhost", "localhost", "localhost" };

        int linesPerHost = PingOutputLikeExpression
                           .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                           .Length;
        int expectedLineCount = linesPerHost * hostNames.Length;

        PingResult result = await Sut.RunAsync(hostNames);

        int actualLineCount = result.StdOutput?
                              .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                              .Length ?? 0;

        Assert.AreEqual(expectedLineCount, actualLineCount);
    }

    [TestMethod]
    async public Task RunLongRunningAsync_UsingTpl_Success()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo("ping", "localhost");
        StringBuilder outputBuilder = new();

        PingResult result = await Sut.RunLongRunningAsync(
            startInfo,
            line => outputBuilder.AppendLine(line), 
            error => { },
            CancellationToken.None
        );

        AssertValidPingOutput(result);
    }

    [TestMethod]
    public void StringBuilderAppendLine_InParallel_IsNotThreadSafe()
    {
        IEnumerable<int> numbers = Enumerable.Range(0, 40);
        System.Text.StringBuilder stringBuilder = new();
        numbers.AsParallel().ForAll(item => stringBuilder.AppendLine(""));
        int lineCount = stringBuilder.ToString().Split(Environment.NewLine).Length;
        Assert.AreNotEqual(lineCount, numbers.Count()+1);
    }


    readonly string PingOutputLikeExpression = @"
Pinging * with 32 bytes of data:
Reply from ::1: time<*
Reply from ::1: time<*
Reply from ::1: time<*
Reply from ::1: time<*

Ping statistics for ::1:
    Packets: Sent = *, Received = *, Lost = 0 (0% loss),
Approximate round trip times in milli-seconds:
    Minimum = *, Maximum = *, Average = *".Trim();
    private void AssertValidPingOutput(int exitCode, string? stdOutput)
    {
        Assert.IsFalse(string.IsNullOrWhiteSpace(stdOutput));
        stdOutput = WildcardPattern.NormalizeLineEndings(stdOutput!.Trim());
        Assert.IsTrue(stdOutput?.IsLike(PingOutputLikeExpression)??false,
            $"Output is unexpected: {stdOutput}");
        Assert.AreEqual<int>(0, exitCode);
    }
    private void AssertValidPingOutput(PingResult result) =>
        AssertValidPingOutput(result.ExitCode, result.StdOutput);
}
