using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using OneNoteDotNet;

namespace OneSchedule
{
    internal class TimestampCollection
    {
        private const string TimestampFormat = "yyyy-MM-ddTHH:mmK";

        private static readonly Regex TimestampRegex =
            new(@"//(?<timestamp>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}(?:Z|[+-]\d{2}:\d{2}))//");

        /// <summary>
        /// A map of OneNote page ID to an <b>ordered</b> (by time) list of timestamps found
        /// on that page.
        /// </summary>
        private readonly Dictionary<string, List<Timestamp>> _timestamps = new();

        // Yes, we're using DateTime to track updates because we're comparing it to page
        // modification times. The assumption is that page modification times are in local time,
        // and not some monotonic clock, so this is exactly what we want. Probably.
        private DateTime _lastUpdateTime = DateTime.MinValue;

        /// <summary>
        /// Updates the collection from OneNote, adding any new timestamps that are
        /// <paramref name="after"/> the specified time, and deleting stale ones.
        /// </summary>
        /// <param name="after">Only timestamps after this time will be added.</param>
        public void Update(DateTime after)
        {
            var oneNote = new OneNote();

            var modifiedTimestamps = FindAllTimestamps(oneNote, _lastUpdateTime, after);
            _lastUpdateTime = DateTime.Now;
            _timestamps.Update(modifiedTimestamps);

            CleanUp(oneNote);
        }

        /// <summary>
        /// Extracts all timestamps <paramref name="until"/> the specified time, and deletes
        /// them from the collection.
        /// </summary>
        /// <param name="until">Only timestamps before this time will be returned.</param>
        public IEnumerable<Timestamp> Remove(DateTime until)
        {
            var toRemove = _timestamps.Values
                .Select(list => list.TakeWhile(timestamp => timestamp.Date <= until))
                .Flatten()
                .ToHashSet();

            foreach (var list in _timestamps.Values)
            {
                list.RemoveAll(timestamp => toRemove.Contains(timestamp));
            }


            _timestamps.RemoveAllKeys(pair => pair.Value.Count == 0);

            return toRemove;
        }

        /// <summary>
        /// Deletes all timestamps that are defined in deleted pages, and deletes all pages
        /// with no timestamps in them from the collection.
        /// </summary>
        private void CleanUp(OneNote oneNote)
        {
            var existingPageIds = oneNote.Hierarchy.AllPages
                .Where(page => !page.IsInRecycleBin)
                .Select(page => page.Id)
                .ToImmutableHashSet();

            _timestamps.RemoveAllKeys(pair => pair.Value.Count <= 0 || !existingPageIds.Contains(pair.Key));
        }

        private static Dictionary<string, List<Timestamp>> FindAllTimestamps(
            OneNote oneNote,
            DateTime pagesModifiedAfter,
            DateTime timestampsAfter
        )
        {
            (string Id, List<Timestamp> Timestamps) PageTimestamps(Page page)
            {
                return (
                    page.Id,
                    FindTimestampsInPage(oneNote.GetPageContent(page.Id, PageInfo.Basic), timestampsAfter).ToList()
                );
            }

            var timestamps = oneNote.Hierarchy.AllPages
                .Where(page => !page.IsInRecycleBin)
                .Where(page => page.LastModifiedTime.GetValueOrDefault(pagesModifiedAfter) >= pagesModifiedAfter)
                .Select(PageTimestamps)
                .ToDictionary(pair => pair.Id, pair => pair.Timestamps);

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

                yield return new Timestamp {Date = timestamp, Comment = remainder};
            }
        }

        private static bool ParseTimestamp(string timestampString, out DateTime timestamp)
        {
            return DateTime.TryParseExact(timestampString, TimestampFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out timestamp);
        }
    }
}
