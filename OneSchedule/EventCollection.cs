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
    internal class EventCollection
    {
        private const string EventTimestampFormat = "yyyy-MM-ddTHH:mmK";

        private static readonly Regex EventTimestampRegex =
            new(@"//(?<timestamp>\d{4}-\d{2}-\d{2}T\d{2}:\d{2}(?:Z|[+-]\d{2}:\d{2}))//");

        /// <summary>
        /// A map of OneNote page ID to an <b>ordered</b> (by time) list of events found
        /// on that page.
        /// </summary>
        private readonly Dictionary<string, LinkedList<Event>> _events = new();

        // Yes, we're using DateTimeOffset to track updates because we're comparing it to page
        // modification times. The assumption is that page modification times are in local time,
        // and not some monotonic clock, so this is exactly what we want. Probably.
        private DateTimeOffset _lastUpdateTime = DateTimeOffset.MinValue;

        private DateTimeOffset _lastNotificationTime = DateTimeOffset.Now;

        public void Notify(Action<Event> callback)
        {
            Update();

            var now = DateTimeOffset.Now;
            foreach (var @event in Remove(now))
            {
                callback.Invoke(@event);
            }

            _lastNotificationTime = now;
        }

        /// <summary>
        /// Updates the collection from OneNote, adding any new events that were
        /// added after the last notification, and deleting stale ones.
        /// </summary>
        private void Update()
        {
            var oneNote = new OneNote();

            var now = DateTimeOffset.Now;
            if (_lastUpdateTime > now)
            {
                // Clock jumped backwards. Rebuild the database just to be safe.
                _events.Clear();
                _lastNotificationTime = DateTimeOffset.MinValue;
                _lastNotificationTime = now;
            }

            var modifiedEvents = FindAllEvents(oneNote, _lastUpdateTime, _lastNotificationTime);
            _lastUpdateTime = DateTimeOffset.Now;
            _events.Update(modifiedEvents);

            CleanUp(oneNote);
        }

        /// <summary>
        /// Extracts all events <paramref name="until"/> the specified time, and deletes
        /// them from the collection.
        /// </summary>
        /// <param name="until">Only events before this time will be returned.</param>
        private IEnumerable<Event> Remove(DateTimeOffset until)
        {
            var toRemove = new List<LinkedListNode<Event>>();

            foreach (var list in _events.Values)
            {
                for (var node = list.First; node != null; node = node.Next)
                {
                    if (node.Value.Date <= until)
                    {
                        toRemove.Add(node);
                    }
                    else
                    {
                        // The lists are sorted, so we can stop early.
                        break;
                    }
                }
            }

            foreach (var node in toRemove)
            {
                Debug.Assert(node.List != null, "node.List != null");
                node.List.Remove(node);
            }

            return toRemove.Select(node => node.Value);
        }

        /// <summary>
        /// Deletes all events that are defined in deleted pages, and deletes all pages
        /// with no events in them from the collection.
        /// </summary>
        private void CleanUp(OneNote oneNote)
        {
            var existingPageIds = oneNote.Hierarchy.AllPages
                .Where(page => !page.IsInRecycleBin)
                .Select(page => page.Id)
                .ToImmutableHashSet();

            _events.RemoveAllKeys(pair => pair.Value.Count <= 0 || !existingPageIds.Contains(pair.Key));
        }

        private static Dictionary<string, LinkedList<Event>> FindAllEvents(
            OneNote oneNote,
            DateTimeOffset pagesModifiedAfter,
            DateTimeOffset eventsAfter
        )
        {
            var events = new Dictionary<string, LinkedList<Event>>(
                oneNote.Hierarchy.AllPages
                    .Where(page => !page.IsInRecycleBin)
                    .Where(page =>
                        page.LastModifiedTime.GetValueOrDefault(pagesModifiedAfter) >= pagesModifiedAfter)
                    .Select(page => new KeyValuePair<string, LinkedList<Event>>(
                        page.Id,
                        new LinkedList<Event>(
                            FindEventsInPage(oneNote.GetPageContent(page.Id, PageInfo.Basic), eventsAfter))
                    ))
            );

            return events;
        }

        private static IEnumerable<Event> FindEventsInPage(PageContent pageContent, DateTimeOffset after)
        {
            return pageContent.PlainTextElements
                .Where(element => !string.IsNullOrWhiteSpace(element))
                .Select(FindEventsInString)
                .Flatten()
                .Where(@event => @event.Date >= after)
                .OrderBy(@event => @event.Date);
        }

        private static IEnumerable<Event> FindEventsInString(string input)
        {
            var (remainder, matches) = EventTimestampRegex.Remove(input);
            remainder = remainder.Trim();
            foreach (Match match in matches)
            {
                Debug.Assert(match.Success);

                if (!ParseEventTimestamp(match.Groups["timestamp"].Value, out var timestamp))
                {
                    continue;
                }

                yield return new Event(timestamp, remainder);
            }
        }

        private static bool ParseEventTimestamp(string timestampString, out DateTimeOffset timestamp)
        {
            return DateTimeOffset.TryParseExact(timestampString, EventTimestampFormat, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out timestamp);
        }
    }
}
