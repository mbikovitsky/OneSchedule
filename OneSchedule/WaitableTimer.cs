using System;
using System.ComponentModel;
using System.Threading;

namespace OneSchedule
{
    internal class WaitableTimer : WaitHandle
    {
        public WaitableTimer(bool manualReset)
        {
            SafeWaitHandle = Native.CreateWaitableTimer(IntPtr.Zero, manualReset, null);
            if (SafeWaitHandle.IsInvalid)
            {
                throw new Win32Exception();
            }
        }

        public void Set(DateTime dueTime, TimeSpan period, bool restore = false)
        {
            var success = Native.SetWaitableTimer(
                SafeWaitHandle,
                DueTimeToLargeInteger(dueTime),
                PeriodToInt(period),
                IntPtr.Zero,
                IntPtr.Zero,
                restore
            );
            if (!success)
            {
                throw new Win32Exception();
            }
        }

        public void Set(TimeSpan dueTime, TimeSpan period, bool restore = false)
        {
            var success = Native.SetWaitableTimer(
                SafeWaitHandle,
                DueTimeToLargeInteger(dueTime),
                PeriodToInt(period),
                IntPtr.Zero,
                IntPtr.Zero,
                restore
            );
            if (!success)
            {
                throw new Win32Exception();
            }
        }

        public void Cancel()
        {
            if (!Native.CancelWaitableTimer(SafeWaitHandle))
            {
                throw new Win32Exception();
            }
        }

        private static Native.LargeInteger DueTimeToLargeInteger(DateTime dueTime)
        {
            return new() {QuadPart = dueTime.ToFileTimeUtc()};
        }

        private static Native.LargeInteger DueTimeToLargeInteger(TimeSpan dueTime)
        {
            var temp = -(dueTime.TotalMilliseconds * 10000);
            if (temp < long.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(dueTime), dueTime, null);
            }

            return new Native.LargeInteger {QuadPart = (long) temp};
        }

        private static int PeriodToInt(TimeSpan period)
        {
            var periodMillis = period.TotalMilliseconds;
            if (periodMillis > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(period), period, null);
            }

            return (int) periodMillis;
        }
    }
}
