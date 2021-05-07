using System;
using System.Runtime.InteropServices;

namespace OneExec
{
    internal static class Native
    {
        [DllImport("shell32.dll", CallingConvention = CallingConvention.Winapi, CharSet = CharSet.Unicode,
            ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr CommandLineToArgvW(string commandLine, out int numArgs);

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        public static extern IntPtr LocalFree(IntPtr memory);
    }
}
