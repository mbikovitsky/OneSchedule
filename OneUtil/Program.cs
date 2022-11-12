using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using Mono.Options;
using OneNoteDotNet;

namespace OneUtil
{
    [SupportedOSPlatform("windows")]
    internal static class Program
    {
        private readonly struct CommandLineOptions
        {
            public bool Hierarchy { get; init; }

            public string? PageId { get; init; }
        }

        private static void Main(string[] args)
        {
            CommandLineOptions? commandLineOptions = ParseCommandLine(args);
            if (commandLineOptions == null)
            {
                return;
            }

            using var application = new Application();

            if (commandLineOptions.Value.Hierarchy)
            {
                Console.WriteLine(application.Hierarchy.Xml);
            }

            if (commandLineOptions.Value.PageId != null)
            {
                Console.WriteLine(application.GetPageContent(commandLineOptions.Value.PageId, PageInfo.All).Xml);
            }
        }

        private static CommandLineOptions? ParseCommandLine(IEnumerable<string> args)
        {
            var showHelp = false;
            var dumpHierarchy = false;
            string? pageId = null;
            var parserConfig = new OptionSet
            {
                { "H|hierarchy", "Dump OneNote XML hierarchy", arg => dumpHierarchy = arg != null },
                { "P|page=", "Dump page contents", arg => pageId = arg },
                { "h|help", "this cruft", arg => showHelp = arg != null },
            };
            parserConfig.Parse(args);

            // ReSharper disable once InvertIf
            if (showHelp)
            {
                string executableName = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine($"Usage: {executableName} [OPTIONS]");
                Console.WriteLine();
                parserConfig.WriteOptionDescriptions(Console.Out);

                return null;
            }

            return new CommandLineOptions { Hierarchy = dumpHierarchy, PageId = pageId };
        }
    }
}
