﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Common;
using Mono.Options;
using OneNoteDotNet;

namespace OneSchedule
{
    internal static class Program
    {
        private static readonly TimeSpan ScanInterval = TimeSpan.FromMinutes(1);

        private readonly struct CommandLineOptions
        {
            public IReadOnlyList<string> Executable { get; init; }
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
            // Since we're scanning every minute of the hour, and since the resolution of
            // the timestamps is also a minute, a single timer is enough here.
            // We don't need to monitor the closest notification time as well.

            using var scanTimer = new WaitableTimer(false);

            scanTimer.Set(DateTime.Now.Ceil(ScanInterval), ScanInterval);

            var timestamps = new Dictionary<string, List<Timestamp>>();
            var lastScanTime = DateTime.MinValue;
            var lastNotificationTime = DateTime.Now;
            while (true)
            {
                var modifiedTimestamps = TimestampExtractor.FindAllTimestamps(lastScanTime, lastNotificationTime);
                lastScanTime = DateTime.Now;
                timestamps.Update(modifiedTimestamps);

                CleanUpDanglingTimestamps(timestamps);

                var now = DateTime.Now;
                Notify(timestamps, now, executable);
                lastNotificationTime = now;

                scanTimer.WaitOne();
            }
        }

        /// <summary>
        /// Starts a notification process for all timestamps <paramref name="until"/> the specified
        /// time, then removes the timestamps from the dictionary.
        /// </summary>
        /// <param name="timestamps">Timestamps to look through</param>
        /// <param name="until">Upper bound on times to send notifications for</param>
        /// <param name="executable">Executable and arguments to launch</param>
        private static void Notify(IDictionary<string, List<Timestamp>> timestamps, DateTime until,
            IReadOnlyList<string> executable)
        {
            var toNotify = timestamps.Values
                .Select(list => list.TakeWhile(timestamp => timestamp.Date <= until))
                .Flatten()
                .ToHashSet();

            foreach (var timestamp in toNotify)
            {
                LaunchNotificationProcess(executable, timestamp);
            }

            foreach (var list in timestamps.Values)
            {
                list.RemoveAll(timestamp => toNotify.Contains(timestamp));
            }


            timestamps.RemoveAllKeys(pair => pair.Value.Count == 0);
        }

        /// <summary>
        /// Launches a process with the given arguments and passes a <see cref="Notification"/>
        /// for the given <paramref name="timestamp"/> over <c>stdin</c>.
        /// </summary>
        /// <param name="executable">Executable and arguments for the process</param>
        /// <param name="timestamp">Timestamp to notify</param>
        private static void LaunchNotificationProcess(IReadOnlyList<string> executable, Timestamp timestamp)
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
                Console.Error.WriteLine($"Failed starting process '{executable[0]}'");
                return;
            }

            var notification = new Notification {Date = timestamp.Date, Comment = timestamp.Comment};

            notification.WriteToStream(process.StandardInput.BaseStream).RunSynchronously();
            process.StandardInput.Close();
        }

        /// <summary>
        /// Deletes all timestamps defined in deleted pages.
        /// </summary>
        /// <param name="timestamps">Timestamps to clean up</param>
        private static void CleanUpDanglingTimestamps(IDictionary<string, List<Timestamp>> timestamps)
        {
            var application = new OneNote();

            var existingPageIds = application.Hierarchy.AllPages.Select(page => page.Id).ToImmutableHashSet();

            timestamps.RemoveAllKeys(pair => !existingPageIds.Contains(pair.Key));
        }
    }
}
