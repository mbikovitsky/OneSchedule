using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OneNoteDotNet;

namespace OneSchedule
{
    internal class Program
    {
        private struct Timestamp
        {
            public DateTime Date;
            public string Comment;
        }

        private static void Main()
        {
            using (var scanTimer = new WaitableTimer(false))
            using (var notificationTimer = new WaitableTimer(false))
            {
                scanTimer.Set(NextNearestMinute(DateTime.Now), TimeSpan.FromMinutes(1));

                WaitHandle[] handles = {notificationTimer, scanTimer};

                while (true)
                {
                    var timestamps = FindClosestTimestamps(DateTime.Now).ToArray();
                    if (timestamps.LongLength > 0)
                    {
                        notificationTimer.Set(timestamps[0].Date, TimeSpan.Zero);
                    }

                    var satisfiedWait = WaitHandle.WaitAny(handles);
                    switch (satisfiedWait)
                    {
                        case 0:
                            foreach (var timestamp in timestamps)
                            {
                                Console.WriteLine($"{timestamp.Date}: {timestamp.Comment}");
                            }

                            break;

                        case 1:
                            // Scan time!
                            break;
                    }
                }
            }
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

        private static IEnumerable<Timestamp> FindClosestTimestamps(DateTime after)
        {
            var oneNote = new OneNote();

            var timestamps = oneNote.Hierarchy.AllPages
                .Select(page => FindTimestampsInPage(oneNote.GetPageContent(page.Id, PageInfo.Basic)))
                .SelectMany(t => t)
                .Where(timestamp => timestamp.Date >= after)
                .OrderBy(timestamp => timestamp.Date)
                .GroupBy(timestamp => timestamp.Date)
                .FirstOrDefault();

            if (timestamps == null)
            {
                return new Timestamp[] { };
            }
            else
            {
                return timestamps;
            }
        }

        private static IEnumerable<Timestamp> FindTimestampsInPage(PageContent pageContent)
        {
            var textElements = pageContent.PlainTextElements.Where(element => !string.IsNullOrWhiteSpace(element));
            foreach (var textElement in textElements)
            {
                var match = Regex.Match(textElement,
                    @"//(?<timestamp>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}(?:Z|[+-]\d{2}:\d{2}))(?:\s(?<comment>.*)|)//");
                if (!match.Success)
                {
                    continue;
                }

                var timestampString = match.Groups["timestamp"].Value;
                var timestamp = DateTime.ParseExact(timestampString, "yyyy-MM-ddTHH:mmK", CultureInfo.InvariantCulture);

                var commentString = match.Groups["comment"].Value.Trim();

                yield return new Timestamp {Comment = commentString, Date = timestamp};
            }
        }
    }
}
