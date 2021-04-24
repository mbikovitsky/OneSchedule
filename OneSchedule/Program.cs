using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace OneSchedule
{
    internal static class Program
    {
        private const double ScanIntervalMinutes = 1;

        private static void Main()
        {
            using (var scanTimer = new WaitableTimer(false))
            using (var notificationTimer = new WaitableTimer(false))
            {
                scanTimer.Set(NextNearestMinute(DateTime.Now), TimeSpan.FromMinutes(ScanIntervalMinutes));

                WaitHandle[] handles = {notificationTimer, scanTimer};

                var timestamps = new Dictionary<string, List<Timestamp>>();
                var lastScanTime = DateTime.MinValue;
                var lastNotificationTime = DateTime.Now;
                while (true)
                {
                    var modifiedTimestamps = TimestampExtractor.FindAllTimestamps(lastScanTime, lastNotificationTime);
                    lastScanTime = DateTime.Now;
                    timestamps.Update(modifiedTimestamps);

                    var closestTimestamp = FindClosestTimestamp(timestamps);
                    notificationTimer.Set(closestTimestamp, TimeSpan.Zero);

                    WaitHandle.WaitAny(handles);

                    var now = DateTime.Now;
                    Notify(timestamps, now);
                    lastNotificationTime = now;
                }
            }
        }

        private static void Notify(IDictionary<string, List<Timestamp>> timestamps, DateTime until)
        {
            var toNotify = timestamps.Values
                .Select(list => list.TakeWhile(timestamp => timestamp.Date <= until))
                .Flatten()
                .ToHashSet();

            foreach (var timestamp in toNotify)
            {
                // TODO: Actual notification
                Console.WriteLine($"{timestamp.Date}: {timestamp.Comment}");
            }

            foreach (var list in timestamps.Values)
            {
                list.RemoveAll(timestamp => toNotify.Contains(timestamp));
            }


            timestamps.RemoveAllKeys(pair => pair.Value.Count == 0);
        }

        private static DateTime FindClosestTimestamp(IReadOnlyDictionary<string, List<Timestamp>> timestamps)
        {
            return timestamps.Values
                .Flatten()
                .Select(timestamp => timestamp.Date)
                .DefaultIfEmpty(DateTime.MaxValue)
                .Min();
        }

        private static DateTime NextNearestMinute(DateTime dateTime)
        {
            if (dateTime.Second == 0 && dateTime.Millisecond == 0)
            {
                return dateTime;
            }

            var nextMinute = dateTime.AddMinutes(1);

            var result = new DateTime(nextMinute.Year, nextMinute.Month, nextMinute.Day, nextMinute.Hour,
                nextMinute.Minute, 0, 0, nextMinute.Kind);

            return result;
        }
    }
}
