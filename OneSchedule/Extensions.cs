using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OneSchedule
{
    internal static class Extensions
    {
        public static (string, MatchCollection) Remove(this Regex regex, string input)
        {
            var matches = regex.Matches(input);
            var remainder = regex.Replace(input, "");
            return (remainder, matches);
        }

        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
        {
            return enumerable.SelectMany(t => t);
        }

        public static void Update<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            IReadOnlyDictionary<TKey, TValue> source
        )
        {
            foreach (var (key, value) in source)
            {
                dictionary[key] = value;
            }
        }

        public static void RemoveAllKeys<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
            Func<KeyValuePair<TKey, TValue>, bool> predicate)
        {
            var toRemove = dictionary.Where(predicate).Select(pair => pair.Key).ToList();
            foreach (var key in toRemove)
            {
                dictionary.Remove(key);
            }
        }

        public static DateTime RoundUpToMinute(this DateTime dateTime)
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
