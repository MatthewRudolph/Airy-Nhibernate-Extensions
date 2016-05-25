using System;
using Dematt.Airy.Nhibernate.NodaTime.Extensions;
using Dematt.Airy.Tests.NodaTime.Entities;
using NHibernate;
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

        [Test]
        public void Can_RoundTrip_A_ZonedDateTime_To_DateTimeOffset_Using_Persistence()
        {
            var systemDateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            Instant now = SystemClock.Instance.Now;
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
