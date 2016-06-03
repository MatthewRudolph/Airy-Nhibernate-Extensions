using System.Linq;
using Dematt.Airy.Nhibernate.NodaTime;
using Dematt.Airy.Tests.NodaTime.Entities;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Util;
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
            ZonedDateTime finishZonedDateTime = startZonedDateTime.Plus(Duration.FromHours(1));

            var testEvent = new ZonedDateTimeTestEntity
            {
                Description = "Can_Write_And_Read_ZonedDateTime_Stored_As_DateTimeOffset",
                StartZonedDateTime = startZonedDateTime,
                FinishZonedDateTime = finishZonedDateTime
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

        /// <summary>
        /// Can we write and read a ZonedDateTime with a null value stored as a DateTimeOffset and a string for the DateTimeZone Id.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_ZonedDateTimeNullable_Stored_As_DateTimeOffset()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            Instant now = SystemClock.Instance.Now;
            ZonedDateTime startZonedDateTime = now.InZone(timeZone);

            var testEvent = new ZonedDateTimeTestEntity
            {
                Description = "Can_Write_And_Read_ZonedDateTimeNullable_Stored_As_DateTimeOffset",
                StartZonedDateTime = startZonedDateTime,
                FinishZonedDateTime = null
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
            Assert.That(retrievedEvent.FinishZonedDateTime, Is.EqualTo(null));
            Assert.That(retrievedEvent, Is.EqualTo(testEvent));
        }

        /// <summary>
        /// Can we query by equals a ZonedDateTime stored as DateTimeOffset and a string for the DateTimeZone Id.
        /// </summary>
        [Test]
        public void Can_Query_By_Equals_ZonedDateTime_Stored_As_DateTimeOffset()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            Instant now = SystemClock.Instance.Now;
            ZonedDateTime startZonedDateTime = now.InZone(timeZone);
            ZonedDateTime finishZonedDateTime = startZonedDateTime.Plus(Duration.FromHours(1));

            var testEvent = new ZonedDateTimeTestEntity
            {
                Description = "Can_Query_By_Equals_ZonedDateTime_Stored_As_DateTimeOffset",
                StartZonedDateTime = startZonedDateTime,
                FinishZonedDateTime = finishZonedDateTime
            };


            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            var searchTime = new ZonedDateTime(finishZonedDateTime.ToInstant(), finishZonedDateTime.Zone);
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<ZonedDateTimeTestEntity>().Where(x => x.FinishZonedDateTime == searchTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }
        }
    }
}
