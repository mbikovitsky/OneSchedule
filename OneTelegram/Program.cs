using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
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

        private const string ChatIdEnvVar = "TELEGRAM_CHAT_ID";

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

        private struct Notification
        {
            public DateTime Date { get; set; }

            public string Comment { get; set; }
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
                await Console.Error.WriteLineAsync($"Missing {TokenEnvVar} environment variable");
                return;
            }

            if (commandLineOptions.Value.DisplayChatIds)
            {
                await DisplayChatIds(token);
            }

            var chatIdString = Environment.GetEnvironmentVariable(ChatIdEnvVar);
            if (null == chatIdString)
            {
                await Console.Error.WriteLineAsync($"Missing {ChatIdEnvVar} environment variable");
                return;
            }

            if (!long.TryParse(chatIdString, out var chatId))
            {
                await Console.Error.WriteLineAsync($"Invalid chat ID specified");
                return;
            }

            await SendNotification(token, chatId);
        }

        private static CommandLineOptions? ParseCommandLine(IEnumerable<string> args)
        {
            var displayChatIds = false;
            var showHelp = false;
            var parserConfig = new OptionSet
            {
                {
                    "d|display-chat-ids",
                    "only display chat IDs of received messages",
                    arg => displayChatIds = arg != null
                },
                {"h|help", "this cruft", arg => showHelp = arg != null},
            };
            parserConfig.Parse(args);

            // ReSharper disable once InvertIf
            if (showHelp)
            {
                var executableName = AppDomain.CurrentDomain.FriendlyName;
                Console.WriteLine($"Usage: {executableName} [OPTIONS]+");
                Console.WriteLine($"The {TokenEnvVar} environment variable must be set to the bot's token.");
                Console.WriteLine($"The {ChatIdEnvVar} must be set for sending notifications.");
                Console.WriteLine();
                parserConfig.WriteOptionDescriptions(Console.Out);

                return null;
            }

            return new CommandLineOptions {DisplayChatIds = displayChatIds};
        }

        /// <summary>
        /// Continuously reads messages sent to the bot on Telegram and prints their chat IDs
        /// to stdout.
        /// </summary>
        /// <param name="token">Telegram bot token</param>
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

        /// <summary>
        /// Sends a notification to Telegram.
        /// </summary>
        /// <param name="token">Telegram bot token</param>
        /// <param name="chatId">Chat ID where to send the notification</param>
        private static async Task SendNotification(string token, long chatId)
        {
            var notification = await ReadNotification();

            var encodedDate = WebUtility.HtmlEncode(notification.Date.ToString("f"));
            var encodedComment = WebUtility.HtmlEncode(notification.Comment);

            var messageHtml = $"<b>{encodedDate}</b>\n{encodedComment}";

            var endpoint = $"https://api.telegram.org/bot{token}/sendMessage";
            var parameters = new {chat_id = chatId, text = messageHtml, parse_mode = "HTML"};

            using var client = new HttpClient();

            var httpResponse = await client.PostAsJsonAsync(endpoint, parameters);
            httpResponse.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Reads a notification structure from stdin.
        /// </summary>
        private static async Task<Notification> ReadNotification()
        {
            await using var stdin = Console.OpenStandardInput();

            await using var memoryStream = new MemoryStream();

            await stdin.CopyToAsync(memoryStream);

            memoryStream.Position = 0;

            var notification = await JsonSerializer.DeserializeAsync<Notification>(memoryStream,
                new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});

            return notification;
        }
    }
}
