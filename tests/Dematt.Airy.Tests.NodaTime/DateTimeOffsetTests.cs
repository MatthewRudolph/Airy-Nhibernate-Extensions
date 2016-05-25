using System;
using Dematt.Airy.Nhibernate.NodaTime.Extensions;
using NodaTime;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaTime
{
    public class DateTimeOffsetTests : PersistenceTest
    {
        [Test]
        public void Can_Create_ZonedDateTime_From_DateTimeOffset_And_DateTimeZone()
        {
            var dateTimeOffsetNow = DateTimeOffset.Now;
            var systemDateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var zonedDateTimeNow = dateTimeOffsetNow.ToZonedDateTime(systemDateTimeZone);

            Assert.That(zonedDateTimeNow, Is.Not.Null);
        }

        [Test]
        public void Can_RoundTrip_A_DateTimeOffset_To_ZonedDateTime_And_Back()
        {
            var dateTimeOffsetNow = DateTimeOffset.Now;
            var systemDateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var zonedDateTimeNow = dateTimeOffsetNow.ToZonedDateTime(systemDateTimeZone);
            var dateTimeOffsetRoundTripped = zonedDateTimeNow.ToDateTimeOffset();

            Assert.That(dateTimeOffsetNow, Is.EqualTo(dateTimeOffsetRoundTripped));
        }

        [Test]
        public void Can_RoundTrip_A_ZonedDateTime_To_DateTimeOffset_And_Back()
        {
            var systemDateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            Instant now = SystemClock.Instance.Now;
            var zonedDateTimeNow = new ZonedDateTime(now, systemDateTimeZone);
            var dateTimeOffsetNow = zonedDateTimeNow.ToDateTimeOffset();
            var zondedDateTimeRoundTripped = dateTimeOffsetNow.ToZonedDateTime(systemDateTimeZone);

            Assert.That(zonedDateTimeNow, Is.EqualTo(zondedDateTimeRoundTripped));
        }
    }
}
