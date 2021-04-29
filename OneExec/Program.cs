using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Common;

namespace OneExec
{
    internal class Program
    {
        private static async Task Main()
        {
            var notification = await Notification.ReadFromStream(Console.OpenStandardInput());

            // The only reason we're doing this argument round-trip is because I can't be bothered
            // passing a gazillion parameters to CreateProcess.

            var arguments = CommandLineToArgv(notification.Comment);

            var startInfo = new ProcessStartInfo
            {
                // https://web.archive.org/web/20110126123911/http://blogs.msdn.com/b/jmstall/archive/2006/09/28/createnowindow.aspx
                FileName = arguments[0],
                CreateNoWindow = false,
                UseShellExecute = false
            };
            foreach (var argument in arguments.Skip(1))
            {
                startInfo.ArgumentList.Add(argument);
            }

            var process = Process.Start(startInfo);
            if (process == null)
            {
                await Console.Error.WriteLineAsync($"Failed starting process '{arguments[0]}'");
                return;
            }
        }

        private static string[] CommandLineToArgv(string commandLine)
        {
            var argv = Native.CommandLineToArgvW(commandLine, out var argc);
            if (argv == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            try
            {
                return Enumerable.Range(0, argc)
                    .Select(index => Marshal.ReadIntPtr(argv + index * IntPtr.Size))
                    .Select(argumentPtr => Marshal.PtrToStringUni(argumentPtr)!)
                    .ToArray();
            }
            finally
            {
                Native.LocalFree(argv);
            }
        }
    }
}
