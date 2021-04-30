using System;

namespace OneSchedule
{
    internal class Event
    {
        public DateTimeOffset Date { get; }

        public string Comment { get; }

        public Event(DateTimeOffset date, string comment)
        {
            Date = date;
            Comment = comment;
        }
    }
}
