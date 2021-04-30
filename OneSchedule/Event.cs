using System;

namespace OneSchedule
{
    internal class Event : IEquatable<Event>
    {
        public DateTimeOffset Date { get; }

        public string Comment { get; }

        public Event(DateTimeOffset date, string comment)
        {
            Date = date;
            Comment = comment;
        }

        public bool Equals(Event? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Date.Equals(other.Date) && Comment == other.Comment;
        }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Event) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Date, Comment);
        }

        public static bool operator ==(Event? left, Event? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Event? left, Event? right)
        {
            return !Equals(left, right);
        }
    }
}
