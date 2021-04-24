using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OneNoteDotNet;

namespace OneSchedule
{
    internal static class Program
    {
        private readonly struct Timestamp : IEquatable<Timestamp>
        {
            public readonly DateTime Date;
            public readonly string Comment;

            public Timestamp(DateTime date, string comment)
            {
                Date = date;
                Comment = comment;
            }

            public bool Equals(Timestamp other)
            {
                return Date.Equals(other.Date) && Comment == other.Comment;
            }

            public override bool Equals(object obj)
            {
                return obj is Timestamp other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Date.GetHashCode() * 397) ^ Comment.GetHashCode();
                }
            }

            public static bool operator ==(Timestamp left, Timestamp right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(Timestamp left, Timestamp right)
            {
                return !left.Equals(right);
            }
        }

        private static readonly Regex TimestampRegex =
            new Regex(@"//(?<timestamp>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}(?:Z|[+-]\d{2}:\d{2}))//");

        private const string TimestampFormat = "yyyy-MM-ddTHH:mmK";

        private static void Main()
        {
            using (var scanTimer = new WaitableTimer(false))
            using (var notificationTimer = new WaitableTimer(false))
            {
                scanTimer.Set(NextNearestMinute(DateTime.Now), TimeSpan.FromMinutes(1));

                WaitHandle[] handles = {notificationTimer, scanTimer};

                var timestamps = new Dictionary<string, List<Timestamp>>();
                var lastScanTime = DateTime.MinValue;
                var lastNotificationTime = DateTime.Now;
                while (true)
                {
                    var modifiedTimestamps = FindAllTimestamps(lastScanTime, lastNotificationTime);
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

        private static Dictionary<string, List<Timestamp>> FindAllTimestamps(
            DateTime pagesModifiedAfter,
            DateTime timestampsAfter
        )
        {
            var oneNote = new OneNote();

            (string Id, List<Timestamp> Timestamps) PageTimestamps(Page page)
            {
                return (
                    page.Id,
                    FindTimestampsInPage(oneNote.GetPageContent(page.Id, PageInfo.Basic), timestampsAfter).ToList()
                );
            }

            var timestamps = oneNote.Hierarchy.AllPages
                .Where(page => page.LastModifiedTime.GetValueOrDefault(pagesModifiedAfter) >= pagesModifiedAfter)
                .Select(PageTimestamps)
                .Where(pair => pair.Timestamps.Count > 0)
                .ToDictionary(pair => pair.Id, tuple => tuple.Timestamps);

            return timestamps;
        }

        private static IEnumerable<Timestamp> FindTimestampsInPage(PageContent pageContent, DateTime after)
        {
            return pageContent.PlainTextElements
                .Where(element => !string.IsNullOrWhiteSpace(element))
                .Select(FindTimestampsInString)
                .Flatten()
                .Where(timestamp => timestamp.Date >= after)
                .OrderBy(timestamp => timestamp.Date);
        }

        private static IEnumerable<Timestamp> FindTimestampsInString(string input)
        {
            var (remainder, matches) = TimestampRegex.Remove(input);
            remainder = remainder.Trim();
            foreach (Match match in matches)
            {
                Debug.Assert(match.Success);

                if (!ParseTimestamp(match.Groups["timestamp"].Value, out var timestamp))
                {
                    continue;
                }

                yield return new Timestamp(timestamp, remainder);
            }
        }

        private static bool ParseTimestamp(string timestampString, out DateTime timestamp)
        {
            return DateTime.TryParseExact(timestampString, TimestampFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out timestamp);
        }

        private static (string, MatchCollection) Remove(this Regex regex, string input)
        {
            var matches = regex.Matches(input);
            var remainder = regex.Replace(input, "");
            return (remainder, matches);
        }

        private static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
        {
            return enumerable.SelectMany(t => t);
        }

        private static void Update<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            IReadOnlyDictionary<TKey, TValue> source
        )
        {
            foreach (var pair in source)
            {
                dictionary[pair.Key] = pair.Value;
            }
        }

        private static void RemoveAllKeys<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            var toRemove = dictionary.Where(predicate).Select(pair => pair.Key).ToList();
            foreach (var key in toRemove)
            {
                dictionary.Remove(key);
            }
        }
    }
}
