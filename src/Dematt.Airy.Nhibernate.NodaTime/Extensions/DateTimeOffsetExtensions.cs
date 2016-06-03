using System;
using NodaTime;
using NodaTime.TimeZones;

namespace Dematt.Airy.Nhibernate.NodaTime.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        public static ZonedDateTime ToZonedDateTime(this DateTimeOffset ext, DateTimeZone dateTimeZone)
        {
            if (dateTimeZone == null)
            {
                throw new DateTimeZoneNotFoundException("Provided DateTimeZone is null. Can not create a ZoneDateTime without a valid DateTimeZone.");
            }
            return new ZonedDateTime(Instant.FromDateTimeOffset(ext), dateTimeZone);
        }

        public static ZonedDateTime? ToZonedDateTime(this DateTimeOffset? ext, DateTimeZone dateTimeZone)
        {
            if (ext == null || dateTimeZone == null)
            {
                return null;
            }
            return new ZonedDateTime(Instant.FromDateTimeOffset(ext.Value), dateTimeZone);
        }
    }
}
