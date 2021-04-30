using System;

namespace OneSchedule
{
    internal readonly struct Timestamp : IEquatable<Timestamp>
    {
        public DateTimeOffset Date { get; init; }

        public string Comment { get; init; }

        public bool Equals(Timestamp other)
        {
            return Date.Equals(other.Date) && Comment == other.Comment;
        }

        public override bool Equals(object? obj)
        {
            return obj is Timestamp other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Date, Comment);
        }

        public static bool operator ==(Timestamp left, Timestamp right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Timestamp left, Timestamp right)
        {
            return !left.Equals(right);
        }
    }
}
