using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Mono.Options;

namespace OneSchedule
{
    internal static class Program
    {
        private const double ScanIntervalMinutes = 1;

        private readonly struct CommandLineOptions
        {
            public readonly List<string> Executable;

            public CommandLineOptions(List<string> executable)
            {
                Executable = executable;
            }
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
            var arguments = new List<string>();
            var showHelp = false;
            var parserConfig = new OptionSet
            {
                {"h|help", "this cruft", arg => showHelp = arg != null},
                {"<>", arg => arguments.Add(arg)},
            };
            parserConfig.Parse(args);

            if (!showHelp && arguments.Count > 0)
            {
                return new CommandLineOptions(arguments);
            }

            var executableName = AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine($"Usage: {executableName} [OPTIONS]+ program [ARGS]+");
            Console.WriteLine();
            parserConfig.WriteOptionDescriptions(Console.Out);

            return null;
        }

        private static void Run(IReadOnlyList<string> executable)
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
                    Notify(timestamps, now, executable);
                    lastNotificationTime = now;
                }
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
                    FileName = executable[0],
                    Arguments = Util.BuildCommandLine(executable.Skip(1)
                        .Concat(new[] {timestamp.Date.ToString("O"), timestamp.Comment})),
                    CreateNoWindow = true,
                    UseShellExecute = false,
                };
                Process.Start(startInfo);
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
