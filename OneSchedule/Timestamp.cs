using System;

namespace OneSchedule
{
    internal class Timestamp : IEquatable<Timestamp>
    {
        public DateTimeOffset Date { get; }

        public string Comment { get; }

        public Timestamp(DateTimeOffset date, string comment)
        {
            Date = date;
            Comment = comment;
        }

        public bool Equals(Timestamp? other)
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
            return Equals((Timestamp) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Date, Comment);
        }

        public static bool operator ==(Timestamp? left, Timestamp? right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Timestamp? left, Timestamp? right)
        {
            return !Equals(left, right);
        }
    }
}
