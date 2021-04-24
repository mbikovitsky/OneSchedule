using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using OneNoteDotNet;

namespace OneSchedule
{
    internal static class TimestampExtractor
    {
        private const string TimestampFormat = "yyyy-MM-ddTHH:mmK";

        private static readonly Regex TimestampRegex =
            new Regex(@"//(?<timestamp>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}(?:Z|[+-]\d{2}:\d{2}))//");

        public static Dictionary<string, List<Timestamp>> FindAllTimestamps(
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
    }
}
