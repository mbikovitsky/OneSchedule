using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common
{
    public class Notification
    {
        public DateTimeOffset Date { get; }

        public string Comment { get; }

        private static readonly JsonSerializerOptions SerializerOptions = new()
            {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

        public Notification(DateTimeOffset date, string comment)
        {
            Date = date;
            Comment = comment;
        }

        public static async Task<Notification> ReadFromStream(Stream stream)
        {
            return (await JsonSerializer.DeserializeAsync<Notification>(stream, SerializerOptions))!;
        }

        public async Task WriteToStream(Stream stream)
        {
            await JsonSerializer.SerializeAsync(stream, this, SerializerOptions);
        }
    }
}
