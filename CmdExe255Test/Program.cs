using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CmdExe255Test {
    internal class Program {
        static void Main(string[] args) {
            CloseHandle(GetStdHandle(STD_INPUT_HANDLE));

            var processStartInfo = new ProcessStartInfo {
                FileName = "cmd",
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                Arguments = @"/c ""Test.bat""",
            };

            Process process = Process.Start(processStartInfo);

            process.ErrorDataReceived += (sender, e) => {
                if (!string.IsNullOrEmpty(e.Data)) {
                    Console.Error.WriteLine($"STDERR: {e.Data}");
                }
            };

            process.OutputDataReceived += (sender, e) => {
                if (!string.IsNullOrEmpty(e.Data)) {
                    Console.Error.WriteLine($"STDOUT: {e.Data}");
                }
            };

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            Console.WriteLine($"Exit code: {process.ExitCode}");
        }

        private const int STD_INPUT_HANDLE = -10;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);
    }
}
