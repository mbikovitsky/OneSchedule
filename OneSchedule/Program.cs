﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Common;
using Mono.Options;

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
            // the event timestamps is also a minute, a single timer is enough here.
            // We don't need to monitor the closest notification time as well.

            using var scanTimer = new WaitableTimer(false);

            scanTimer.Set(DateTimeOffset.Now.Ceil(ScanInterval), ScanInterval);

            var collection = new EventCollection();
            while (true)
            {
                collection.Notify(@event =>
                {
                    Console.WriteLine($"{@event.Date:g} - {@event.Comment}");
                    LaunchNotificationProcess(executable, @event);
                });

                scanTimer.WaitOne();
            }
        }

        /// <summary>
        /// Launches a process with the given arguments and passes a <see cref="Notification"/>
        /// for the given <paramref name="event"/> over <c>stdin</c>.
        /// </summary>
        /// <param name="executable">Executable and arguments for the process</param>
        /// <param name="event">Event to notify about</param>
        private static void LaunchNotificationProcess(IReadOnlyList<string> executable, Event @event)
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

            var notification = new Notification(@event.Date, @event.Comment);

            notification.WriteToStream(process.StandardInput.BaseStream).Wait();
            process.StandardInput.Close();
        }
    }
}
