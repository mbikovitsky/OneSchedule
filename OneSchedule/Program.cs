using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading;
using Mono.Options;

namespace OneSchedule
{
    internal static class Program
    {
        private const double ScanIntervalMinutes = 1;

        private readonly struct CommandLineOptions
        {
            public IReadOnlyList<string> Executable { get; init; }
        }

        private struct Notification
        {
            public DateTime Date { get; set; }

            public string Comment { get; set; }
        }

        private static void Main(string[] args)
        {
            var commandLineOptions = ParseCommandLine(args);
            if (commandLineOptions == null)
            {
                return;
            }

            Run(commandLineOptions.Value.Executable);
        }

        private static CommandLineOptions? ParseCommandLine(IEnumerable<string> args)
        {
            var showHelp = false;
            var parserConfig = new OptionSet
            {
                {"h|help", "this cruft", arg => showHelp = arg != null},
            };
            var executableArguments = parserConfig.Parse(args);

            // ReSharper disable once InvertIf
            if (showHelp || executableArguments.Count <= 0)
            {
                var executableName = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine($"Usage: {executableName} [OPTIONS]+ program [ARGS]+");
                Console.WriteLine();
                parserConfig.WriteOptionDescriptions(Console.Out);

                return null;
            }

            return new CommandLineOptions {Executable = executableArguments};
        }

        private static void Run(IReadOnlyList<string> executable)
        {
            using var scanTimer = new WaitableTimer(false);
            using var notificationTimer = new WaitableTimer(false);

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
                Notify(timestamps, now, executable);
                lastNotificationTime = now;
            }
        }

        private static void Notify(IDictionary<string, List<Timestamp>> timestamps, DateTime until,
            IReadOnlyList<string> executable)
        {
            var toNotify = timestamps.Values
                .Select(list => list.TakeWhile(timestamp => timestamp.Date <= until))
                .Flatten()
                .ToHashSet();

            foreach (var timestamp in toNotify)
            {
                var startInfo = new ProcessStartInfo
                {
                    // https://web.archive.org/web/20110126123911/http://blogs.msdn.com/b/jmstall/archive/2006/09/28/createnowindow.aspx
                    FileName = executable[0],
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    RedirectStandardInput = true
                };
                foreach (var argument in executable.Skip(1))
                {
                    startInfo.ArgumentList.Add(argument);
                }

                var process = Process.Start(startInfo);
                if (process == null)
                {
                    continue;
                }

                var notification = new Notification {Date = timestamp.Date, Comment = timestamp.Comment};

                using var writer = new Utf8JsonWriter(process.StandardInput.BaseStream);
                JsonSerializer.Serialize(writer, notification,
                    new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
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
