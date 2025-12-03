using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Assignment;

public record struct PingResult(int ExitCode, string? StdOutput);

public class PingProcess
{
    private ProcessStartInfo StartInfo { get; } = new("ping");

    public virtual PingResult Run(string hostNameOrAddress)
    {
        string args = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "-n 4" : "-c 4";

        StartInfo.Arguments = hostNameOrAddress + " " + args;

        StringBuilder? stringBuilder = null;
        void updateStdOutput(string? line) =>
            (stringBuilder ??= new StringBuilder()).AppendLine(line);

        Process process = RunProcessInternal(StartInfo, updateStdOutput, default, default);

        int exitCode = process.ExitCode;
        if (exitCode != 0) exitCode = 1;

        return new PingResult(exitCode, stringBuilder?.ToString());
    }

    public Task<PingResult> RunTaskAsync(string hostNameOrAddress)
    {
        return Task.Run(() =>
        {
            return Run(hostNameOrAddress);
        });
    }

    async public Task<PingResult> RunAsync(string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        Task<PingResult> task = RunTaskAsync(hostNameOrAddress);
        PingResult result = await task.WaitAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return result;
    }

    async public Task<PingResult> RunAsync(IEnumerable<string> hostNameOrAddresses, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(hostNameOrAddresses);

        StringBuilder stringBuilder = new();
        object lockObject = new();

        IEnumerable<Task<int>> tasks = hostNameOrAddresses.Select(host => Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            PingResult result = Run(host);

            string output = result.StdOutput?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(output))
            {
                lock (lockObject)
                {
                    stringBuilder.AppendLine(output);
                }
            }

            return result.ExitCode;
        }, cancellationToken)).ToList();

        int[] results = await Task.WhenAll(tasks);
        int totalExitCode = results.Sum();
        return new PingResult(totalExitCode, stringBuilder.ToString());
    }

    public Task<PingResult> RunLongRunningAsync(
        ProcessStartInfo startInfo, Action<string?>? progressOutput,
        Action<string?>? progressError, CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(startInfo);

        Task<PingResult> task = Task.Factory.StartNew(() =>
        {
            token.ThrowIfCancellationRequested();

            StringBuilder stringBuilder = new();
            void captureOutput(string? line)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    lock (stringBuilder)
                    {
                        stringBuilder.AppendLine(line);
                    }
                }

                progressOutput?.Invoke(line);
            }

            void captureError(string? line)
            {
                progressError?.Invoke(line);
            }

            Process process = RunProcessInternal(startInfo, captureOutput, captureError, token);

            int exitCode = process.ExitCode;
            if (exitCode != 0) exitCode = 1;

            string? output = stringBuilder.ToString();

            return new PingResult(exitCode, output);

        }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);

        return task;
    }

    // extra credit
    public async Task<PingResult> RunAsync(IProgress<string?> progress)
    {
        ArgumentNullException.ThrowIfNull(progress);

        StringBuilder outputBuilder = new();
        void captureLine(string? line)
        {
            if (line is null)
                return;

            lock (outputBuilder)
            {
                outputBuilder.AppendLine(line);
            }

            progress.Report(line);
        }

        return await Task.Run(() =>
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("ping", "localhost");
            Process process = RunProcessInternal(startInfo, captureLine, null, default);

            int exitCode = process.ExitCode;
            if (exitCode != 0) exitCode = 1; // normalize exit code

            return new PingResult(exitCode, outputBuilder.ToString());
        });
    }

    private Process RunProcessInternal(
        ProcessStartInfo startInfo,
        Action<string?>? progressOutput,
        Action<string?>? progressError,
        CancellationToken token)
    {
        var process = new Process
        {
            StartInfo = UpdateProcessStartInfo(startInfo)
        };
        return RunProcessInternal(process, progressOutput, progressError, token);
    }

    private Process RunProcessInternal(
        Process process,
        Action<string?>? progressOutput,
        Action<string?>? progressError,
        CancellationToken token)
    {
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += OutputHandler;
        process.ErrorDataReceived += ErrorHandler;

        try
        {
            if (!process.Start())
            {
                return process;
            }

            token.Register(obj =>
            {
                if (obj is Process p && !p.HasExited)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch (Win32Exception ex)
                    {
                        throw new InvalidOperationException($"Error cancelling process{Environment.NewLine}{ex}");
                    }
                }
            }, process);

            if (process.StartInfo.RedirectStandardOutput)
                process.BeginOutputReadLine();
            if (process.StartInfo.RedirectStandardError)
                process.BeginErrorReadLine();

            if (process.HasExited)
            {
                return process;
            }
                process.WaitForExit();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Error running '{process.StartInfo.FileName} {process.StartInfo.Arguments}'{Environment.NewLine}{e}");
        }
        finally
        {
            if (process.StartInfo.RedirectStandardError)
                process.CancelErrorRead();
            if (process.StartInfo.RedirectStandardOutput)
                process.CancelOutputRead();
            
            process.OutputDataReceived -= OutputHandler;
            process.ErrorDataReceived -= ErrorHandler;

            if (!process.HasExited)
                process.Kill();
        }

        return process;

        void OutputHandler(object s, DataReceivedEventArgs e) => progressOutput?.Invoke(e.Data);
        void ErrorHandler(object s, DataReceivedEventArgs e) => progressError?.Invoke(e.Data);
    }

    private static ProcessStartInfo UpdateProcessStartInfo(ProcessStartInfo startInfo)
    {
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardError = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;

        return startInfo;
    }
}
