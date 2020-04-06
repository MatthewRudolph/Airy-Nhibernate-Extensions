using System;
using Dematt.Airy.Nhibernate.NodaTime.Extensions;
using Dematt.Airy.Tests.NodaTime.Entities;
using NHibernate;
using NodaTime;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaTime
{
    /// <summary>
    /// Tests for using a Bcl DateTimeOffset with a separate NodaTime DateTimeZone field 
    /// to store the full date time with offset and time zone that an event happened.
    /// </summary>
    /// <remarks>
    /// This is useful over storing when the event happened using a NodaTime ZonedDateTime as 
    /// it allows use to more easily query for events using a *.Now time. We can get a UtcNow 
    /// and get all events that it is between regardless of their time zone or offset.
    /// 
    /// This may be possible using ZonedDateTime but it would at the least require that it 
    /// be stored in the database as a Int64 for the instant and string for the DateTimeZone Id and will require further investigation.
    /// </remarks>
    public class DateTimeOffsetTests : PersistenceTest
    {
        /// <summary>
        /// Can we create a ZonedDateTime from a DateTimeOffset and a DateTimeZone.
        /// </summary>
        [Test]
        public void Can_Create_ZonedDateTime_From_DateTimeOffset_And_DateTimeZone()
        {
            var dateTimeOffsetNow = DateTimeOffset.Now;
            var systemDateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var zonedDateTimeNow = dateTimeOffsetNow.ToZonedDateTime(systemDateTimeZone);

            Assert.That(zonedDateTimeNow, Is.Not.Null);
        }

        /// <summary>
        /// Can we round-trip a DateTimeOffset with a DateTimeZone to a ZonedDateTime and get the same value back.
        /// </summary>
        [Test]
        public void Can_RoundTrip_A_DateTimeOffset_To_ZonedDateTime_And_Back()
        {
            var dateTimeOffsetNow = DateTimeOffset.Now;
            var systemDateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var zonedDateTimeNow = dateTimeOffsetNow.ToZonedDateTime(systemDateTimeZone);
            var dateTimeOffsetRoundTripped = zonedDateTimeNow.ToDateTimeOffset();

            Assert.That(dateTimeOffsetNow, Is.EqualTo(dateTimeOffsetRoundTripped));
        }

        /// <summary>
        /// Can we round-trip a ZonedDateTime to a DateTimeOffset and DateTimeZone and get the ame value back.
        /// </summary>
        [Test]
        public void Can_RoundTrip_A_ZonedDateTime_To_DateTimeOffset_And_Back()
        {
            var systemDateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            Instant now = TestClock.GetCurrentInstant();
            var zonedDateTimeNow = new ZonedDateTime(now, systemDateTimeZone);
            var dateTimeOffsetNow = zonedDateTimeNow.ToDateTimeOffset();
            var zondedDateTimeRoundTripped = dateTimeOffsetNow.ToZonedDateTime(systemDateTimeZone);

            Assert.That(zonedDateTimeNow, Is.EqualTo(zondedDateTimeRoundTripped));
        }

        /// <summary>
        /// Can we round-trip a ZonedDateTime to a DateTimeOffset and DateTimeZone
        /// to a database using NHibernate and get the same value back.
        /// </summary>
        [Test]
        public void Can_RoundTrip_A_ZonedDateTime_To_DateTimeOffset_Using_Persistence()
        {
            var systemDateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            Instant now = TestClock.GetCurrentInstant();
            var zonedDateTimeStart = new ZonedDateTime(now, systemDateTimeZone);
            var zonedDateTimeFinish = zonedDateTimeStart.Plus(Duration.FromMinutes(60));

            var testLocation = new LocationTestEntity
            {
                Description = "Can_RoundTrip_A_ZonedDateTime_To_DateTimeOffset_Using_Persistence",
                LocationDateTimeZone = systemDateTimeZone
            };

            var testEvent = new DateTimeOffsetTestEntity
            {
                Description = "Can_RoundTrip_A_ZonedDateTime_To_DateTimeOffset_Using_Persistence",
                EventLocation = testLocation,
                StartDateTimeOffset = zonedDateTimeStart.ToDateTimeOffset(),
                FinishDateTimeOffset = zonedDateTimeFinish.ToDateTimeOffset()
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testLocation);
                session.Save(testEvent);
                transaction.Commit();
            }

            ZonedDateTime retrievedZonedDateStart;
            ZonedDateTime retrievedZonedDateFinish;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var retrievedEvent = session.Get<DateTimeOffsetTestEntity>(testEvent.Id);
                retrievedZonedDateStart =
                    retrievedEvent.StartDateTimeOffset.ToZonedDateTime(retrievedEvent.EventLocation.LocationDateTimeZone);
                retrievedZonedDateFinish =
                    retrievedEvent.FinishDateTimeOffset.ToZonedDateTime(retrievedEvent.EventLocation.LocationDateTimeZone);
                transaction.Commit();
            }

            Assert.That(zonedDateTimeStart, Is.EqualTo(retrievedZonedDateStart));
            Assert.That(zonedDateTimeFinish, Is.EqualTo(retrievedZonedDateFinish));
        }
    }
}
