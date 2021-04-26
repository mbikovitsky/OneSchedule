using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Mono.Options;

namespace OneTelegram
{
    internal static class Program
    {
        private const string TokenEnvVar = "TELEGRAM_TOKEN";

        private static readonly TimeSpan UpdateInterval = new(0, 0, 0, 1);

        private readonly struct CommandLineOptions
        {
            public bool DisplayChatIds { get; init; }
        }

        private struct GetUpdatesResponse
        {
            public bool ok { get; set; }

            public Update[]? result { get; set; }
        }

        private struct Update
        {
            public long update_id { get; set; }

            public Message? message { get; set; }
        }

        private struct Message
        {
            public Chat chat { get; set; }
        }

        private struct Chat
        {
            public long id { get; set; }
        }

        private static async Task Main(string[] args)
        {
            var commandLineOptions = ParseCommandLine(args);
            if (null == commandLineOptions)
            {
                return;
            }

            var token = Environment.GetEnvironmentVariable(TokenEnvVar);
            if (null == token)
            {
                await Console.Error.WriteLineAsync("Missing TOKEN environment variable");
                return;
            }

            if (commandLineOptions.Value.DisplayChatIds)
            {
                await DisplayChatIds(token);
            }
        }

        private static CommandLineOptions? ParseCommandLine(IEnumerable<string> args)
        {
            var displayChatIds = false;
            var showHelp = false;
            var parserConfig = new OptionSet
            {
                {"c|chat-ids", "only display chat IDs of received messages", arg => displayChatIds = arg != null},
                {"h|help", "this cruft", arg => showHelp = arg != null},
            };
            parserConfig.Parse(args);

            // ReSharper disable once InvertIf
            if (showHelp)
            {
                var executableName = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine($"Usage: {executableName} [OPTIONS]+");
                Console.WriteLine();
                parserConfig.WriteOptionDescriptions(Console.Out);

                return null;
            }

            return new CommandLineOptions {DisplayChatIds = displayChatIds};
        }

        private static async Task DisplayChatIds(string token)
        {
            var endpoint = $"https://api.telegram.org/bot{token}/getUpdates";

            using var client = new HttpClient();

            long? lastUpdateId = null;
            while (true)
            {
                var parameters = new {offset = lastUpdateId + 1};

                var httpResponse = await client.PostAsJsonAsync(endpoint, parameters,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web) {IgnoreNullValues = true});
                httpResponse.EnsureSuccessStatusCode();

                var getUpdatesResponse = await httpResponse.Content.ReadFromJsonAsync<GetUpdatesResponse>();
                if (getUpdatesResponse.ok)
                {
                    Debug.Assert(getUpdatesResponse.result != null, "update.result != null");

                    foreach (var update in getUpdatesResponse.result)
                    {
                        if (lastUpdateId == null || update.update_id > lastUpdateId)
                        {
                            lastUpdateId = update.update_id;
                        }

                        if (update.message != null)
                        {
                            Console.WriteLine(update.message.Value.chat.id);
                        }
                    }
                }

                await Task.Delay(UpdateInterval);
            }
        }
    }
}
