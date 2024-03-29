﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using Common;
using Mono.Options;
using OneNoteDotNet;

namespace OneSchedule
{
    [SupportedOSPlatform("windows")]
    internal static class Program
    {
        private static readonly TimeSpan ScanInterval = TimeSpan.FromMinutes(1);

        private const int DefaultFullScanIntervalMinutes = 30;

        private readonly struct CommandLineOptions
        {
            public IReadOnlyList<string> Executable { get; init; }

            public bool Silent { get; init; }

            public TimeSpan FullScanInterval { get; init; }

            public bool ShowNotebooks { get; init; }

            public string NotebookToMonitor { get; init; }
        }

        private static void Main(string[] args)
        {
            var commandLineOptions = ParseCommandLine(args);
            if (commandLineOptions == null)
            {
                return;
            }

            if (commandLineOptions.Value.ShowNotebooks)
            {
                foreach (Notebook notebook in new Application().Hierarchy.Notebooks)
                {
                    Console.WriteLine($"{notebook.Name} - {notebook.Id}");
                }

                return;
            }

            Run(commandLineOptions.Value.Executable, commandLineOptions.Value.Silent,
                commandLineOptions.Value.FullScanInterval, commandLineOptions.Value.NotebookToMonitor);
        }

        private static CommandLineOptions? ParseCommandLine(IEnumerable<string> args)
        {
            var showHelp = false;
            var silent = false;
            TimeSpan fullScanInterval = TimeSpan.FromMinutes(DefaultFullScanIntervalMinutes);
            var showNotebooks = false;
            string notebookToMonitor = "";
            var parserConfig = new OptionSet
            {
                { "s|silent", "do not display a console window", arg => silent = arg != null },
                {
                    "f|full-scan=", $"interval for full scans, in minutes (default: {DefaultFullScanIntervalMinutes})",
                    (int minutes) => fullScanInterval = TimeSpan.FromMinutes(minutes)
                },
                { "n|notebooks", "show a list of all notebooks and quit", arg => showNotebooks = arg != null },
                {
                    "m|monitor=", "ID of a notebook to monitor (default is to monitor everything)",
                    arg => notebookToMonitor = arg
                },
                { "h|help", "this cruft", arg => showHelp = arg != null },
            };
            var executableArguments = parserConfig.Parse(args);

            // ReSharper disable once InvertIf
            if (showHelp || (executableArguments.Count <= 0 && !showNotebooks))
            {
                var executableName = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine($"Usage: {executableName} [OPTIONS]+ program [ARGS]+");
                Console.WriteLine();
                parserConfig.WriteOptionDescriptions(Console.Out);

                return null;
            }

            return new CommandLineOptions
            {
                Executable = executableArguments, Silent = silent, FullScanInterval = fullScanInterval,
                ShowNotebooks = showNotebooks, NotebookToMonitor = notebookToMonitor
            };
        }

        private static void Run(IReadOnlyList<string> executable, bool silent, TimeSpan fullScanInterval,
            string notebookToMonitor)
        {
            if (silent)
            {
                Native.FreeConsole();
            }

            var fullScanStopwatch = new Stopwatch();

            var collection = new EventCollection(notebookToMonitor);
            fullScanStopwatch.Start();
            while (true)
            {
                collection.Notify(@event =>
                {
                    Console.WriteLine($"{@event.Date:yyyy-MM-ddTHH:mmK} - {@event.Comment}");
                    LaunchNotificationProcess(executable, @event, silent);
                });

                Sleep();

                if (fullScanStopwatch.Elapsed < fullScanInterval)
                {
                    continue;
                }

                collection = new EventCollection(notebookToMonitor);
                fullScanStopwatch.Restart();
            }
        }

        private static void Sleep()
        {
            var now = DateTimeOffset.Now;
            var toSleep = now.Ceil(ScanInterval) - now;
            Thread.Sleep(toSleep);
        }

        /// <summary>
        /// Launches a process with the given arguments and passes a <see cref="Notification"/>
        /// for the given <paramref name="event"/> over <c>stdin</c>.
        /// </summary>
        /// <param name="executable">Executable and arguments for the process</param>
        /// <param name="event">Event to notify about</param>
        /// <param name="silent">Indicates whether the child process should be created without a console window</param>
        private static void LaunchNotificationProcess(IReadOnlyList<string> executable, Event @event, bool silent)
        {
            var startInfo = new ProcessStartInfo
            {
                // https://web.archive.org/web/20110126123911/http://blogs.msdn.com/b/jmstall/archive/2006/09/28/createnowindow.aspx
                FileName = executable[0],
                CreateNoWindow = silent,
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
