using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Common
{
    public struct Notification
    {
        public DateTime Date { get; set; }

        public string Comment { get; set; }

        private static readonly JsonSerializerOptions SerializerOptions = new()
            {PropertyNamingPolicy = JsonNamingPolicy.CamelCase};

        /// <summary>
        /// Reads a notification structure from stdin.
        /// </summary>
        public static async Task<Notification> ReadFromStream(Stream stream)
        {
            return await JsonSerializer.DeserializeAsync<Notification>(stream, SerializerOptions);
        }

        public async Task WriteToStream(Stream stream)
        {
            await JsonSerializer.SerializeAsync(stream, this, SerializerOptions);
        }
    }
}
