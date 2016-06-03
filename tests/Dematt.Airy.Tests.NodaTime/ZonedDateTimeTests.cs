using System;
using Dematt.Airy.Nhibernate.NodaTime;
using Dematt.Airy.Tests.NodaTime.Entities;
using NHibernate;
using NodaTime;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaTime
{
    /// <summary>
    /// Tests for persisting the NodaTime  <see cref="ZonedDateTime"/> type using the <see cref="ZonedDateTimeType"/> NHibernate custom user type.
    /// </summary>
    public class ZonedDateTimeTests : PersistenceTest
    {
        /// <summary>
        /// Can we write a ZonedDateTime stored as a DateTimeOffset and a string for the DateTimeZone Id.
        /// </summary>
        [Test]
        public void Can_Write_ZonedDateTime_Stored_As_DateTimeOffset()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
                Instant now = SystemClock.Instance.Now;
                ZonedDateTime startZonedDateTime = now.InZone(timeZone);

                var testEvent = new ZonedDateTimeTestEntity
                {
                    Description = "Can_Write_ZonedDateTime_Stored_As_DateTimeOffset",
                    StartZonedDateTime = startZonedDateTime
                };
                session.Save(testEvent);
                transaction.Commit();

                Assert.That(testEvent.Id, Is.Not.Null);
            }
        }


        /// <summary>
        /// Can we write and read a ZonedDateTime stored as a DateTimeOffset and a string for the DateTimeZone Id.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_ZonedDateTime_Stored_As_DateTimeOffset()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            Instant now = SystemClock.Instance.Now;
            ZonedDateTime startZonedDateTime = now.InZone(timeZone);
            ZonedDateTime finihshZonedDateTime = startZonedDateTime.Plus(Duration.FromHours(1));

            var testEvent = new ZonedDateTimeTestEntity
            {
                Description = "Can_Write_And_Read_OffsetDateTime_Stored_As_DateTimeOffset",
                StartZonedDateTime = startZonedDateTime,
                FinishZonedDateTime = finihshZonedDateTime
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            ZonedDateTimeTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<ZonedDateTimeTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent.StartZonedDateTime, Is.EqualTo(testEvent.StartZonedDateTime));
            Assert.That(retrievedEvent.FinishZonedDateTime, Is.EqualTo(testEvent.FinishZonedDateTime));
            Assert.That(retrievedEvent, Is.EqualTo(testEvent));
        }
    }
}
