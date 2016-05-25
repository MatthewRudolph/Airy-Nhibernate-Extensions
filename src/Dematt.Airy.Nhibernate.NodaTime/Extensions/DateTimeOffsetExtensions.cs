using System;
using NodaTime;

namespace Dematt.Airy.Nhibernate.NodaTime.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static ZonedDateTime ToZonedDateTime(this DateTimeOffset ext, DateTimeZone dateTimeZone)
        {
            return new ZonedDateTime(Instant.FromDateTimeOffset(ext), dateTimeZone);
        }
    }
}
