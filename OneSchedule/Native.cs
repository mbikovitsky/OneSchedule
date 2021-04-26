using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace OneSchedule
{
    internal class Native
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct LargeInteger
        {
            [FieldOffset(0)] public uint LowPart;

            [FieldOffset(4)] public int HighPart;

            [FieldOffset(0)] public long QuadPart;
        }

        [DllImport("kernel32.dll",
            CallingConvention = CallingConvention.Winapi,
            CharSet = CharSet.Unicode,
            SetLastError = true)]
        public static extern SafeWaitHandle CreateWaitableTimer(
            IntPtr timerAttributes,
            [MarshalAs(UnmanagedType.Bool)] bool manualReset,
            string? timerName
        );

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWaitableTimer(
            SafeWaitHandle timer,
            in LargeInteger dueTime,
            int period,
            IntPtr completionRoutine,
            IntPtr argToCompletionRoutine,
            [MarshalAs(UnmanagedType.Bool)] bool resume
        );

        [DllImport("kernel32.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CancelWaitableTimer(SafeWaitHandle timer);
    }
}
