using System.Runtime.InteropServices;

namespace OneSchedule
{
    internal static class Native
    {
        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeConsole();
    }
}
