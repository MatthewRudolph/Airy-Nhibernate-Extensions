using System;
using System.Linq;
using Dematt.Airy.Nhibernate.NodaTime;
using Dematt.Airy.Tests.NodaTime.Entities;
using NHibernate;
using NHibernate.Linq;
using NodaTime;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaTime
{
    /// <summary>
    /// Tests for persisting the NodaTime  <see cref="OffsetDateTime"/> type using the <see cref="OffsetDateTimeType"/> NHibernate custom user type.
    /// </summary>
    public class OffsetDateTimeTests : PersistenceTest
    {
        /// <summary>
        /// Can we write a OffsetDateTime stored as a DateTimeOffset.
        /// </summary>
        [Test]
        public void Can_Write_OffsetDateTime_Stored_As_DateTimeOffset()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
                Instant now = SystemClock.Instance.Now;
                ZonedDateTime zonedNowDateTime = now.InZone(timeZone);

                var testEvent = new OffsetDateTimeTestEntity
                {
                    Description = "Can_Write_OffsetDateTime_Stored_As_DateTimeOffset",
                    SystemDateTimeOffset = DateTimeOffset.Now,
                    StartOffsetDateTime = new OffsetDateTime(zonedNowDateTime.LocalDateTime, zonedNowDateTime.Offset)
                };
                session.Save(testEvent);
                transaction.Commit();

                Assert.That(testEvent.Id, Is.Not.Null);
            }
        }

        /// <summary>
        /// Can we write a calculated OffsetDateTime stored as a DateTimeOffset.
        /// </summary>
        [Test]
        public void Can_Write_Calculated_OffsetDateTime_Stored_As_DateTimeOffset()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
                Instant now = SystemClock.Instance.Now;
                ZonedDateTime zonedNowDateTime = now.InZone(timeZone);
                var zonedFinishDateTime = zonedNowDateTime.Plus(Duration.FromMinutes(60));

                var testEvent = new OffsetDateTimeTestEntity
                {
                    Description = "Can_Write_Calculated_OffsetDateTime_Stored_As_DateTimeOffset",
                    SystemDateTimeOffset = DateTimeOffset.Now,
                    StartOffsetDateTime = new OffsetDateTime(zonedNowDateTime.LocalDateTime, zonedNowDateTime.Offset),
                    FinishOffsetDateTime =
                        new OffsetDateTime(zonedFinishDateTime.LocalDateTime, zonedFinishDateTime.Offset)
                };
                session.Save(testEvent);
                transaction.Commit();

                Assert.That(testEvent.Id, Is.Not.Null);
            }
        }

        /// <summary>
        /// Can we write and read a OffsetDateTime stored as a DateTimeOffset.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_OffsetDateTime_Stored_As_DateTimeOffset()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            Instant now = SystemClock.Instance.Now;
            ZonedDateTime zonedNowDateTime = now.InZone(timeZone);
            var testEvent = new OffsetDateTimeTestEntity
            {
                Description = "Can_Write_And_Read_OffsetDateTime_Stored_As_DateTimeOffset",
                SystemDateTimeOffset = DateTimeOffset.Now,
                StartOffsetDateTime = new OffsetDateTime(zonedNowDateTime.LocalDateTime, zonedNowDateTime.Offset)
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            OffsetDateTimeTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<OffsetDateTimeTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent, Is.EqualTo(testEvent));
        }

        /// <summary>
        /// Can we query by equals a OffsetDateTime stored as DateTimeOffset.
        /// </summary>
        /// <remarks>
        /// WARNING: Microsoft SQL Server and the BCL have different equals logic of DateTimeOffsets compared to that if the NodaTime OffsetDateTime equals logic.
        /// For Microsoft SQL Server and the BCL (DateTimeOffset):
        ///     Before it performs the comparison, both DateTimeOffset objects are converted to Coordinated Universal Time (UTC). 
        ///     This is equivalent to the following: return first.UtcDateTime == second.UtcDateTime;
        /// For NodaTime (OffsetDateTime)
        ///     Both the LocalDateTime and the Offset must both be the same for two OffsetDateTime objects to be considered equals.
        ///     This is true even if the two objects even though having different LocalDateTime and the Offset values point to the same Instant in time.
        /// This means that Linq/SQL queries may give different results to C# compressions of objects.
        /// </remarks>
        [Test]
        public void Can_Query_By_Equals_OffsetDateTime_Stored_As_DateTimeOffset()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            Instant now = SystemClock.Instance.Now;
            ZonedDateTime zonedNowDateTime = now.InZone(timeZone);

            var zonedFinishDateTime = zonedNowDateTime.Plus(Duration.FromMinutes(60));
            var offsetFinishTime = new OffsetDateTime(zonedFinishDateTime.LocalDateTime, zonedFinishDateTime.Offset);

            var testEvent = new OffsetDateTimeTestEntity
                {
                    Description = "Can_Query_By_Equals_OffsetDateTime_Stored_As_DateTimeOffset",
                    SystemDateTimeOffset = DateTimeOffset.Now,
                    StartOffsetDateTime = new OffsetDateTime(zonedNowDateTime.LocalDateTime, zonedNowDateTime.Offset),
                    FinishOffsetDateTime = offsetFinishTime
                };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<OffsetDateTimeTestEntity>().Where(x => x.FinishOffsetDateTime == offsetFinishTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }
        }

        /// <summary>
        /// Can we query by equals a OffsetDateTime stored as DateTimeOffset using a UTC time.
        /// </summary>
        /// <remarks>
        /// WARNING: Microsoft SQL Server and the BCL have different equals logic of DateTimeOffsets compared to that if the NodaTime OffsetDateTime equals logic.
        /// For Microsoft SQL Server and the BCL (DateTimeOffset):
        ///     Before it performs the comparison, both DateTimeOffset objects are converted to Coordinated Universal Time (UTC). 
        ///     This is equivalent to the following: return first.UtcDateTime == second.UtcDateTime;
        /// For NodaTime (OffsetDateTime)
        ///     Both the LocalDateTime and the Offset must both be the same for two OffsetDateTime objects to be considered equals.
        ///     This is true even if the two objects even though having different LocalDateTime and the Offset values point to the same Instant in time.
        /// This means that Linq/SQL queries may give different results to C# compressions of objects.
        /// </remarks>
        [Test]
        public void Can_Query_By_Equals_Using_A_UtcTime_OffsetDateTime_Stored_As_DateTimeOffset()
        {
            var timeZoneUnitedKingdom = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/London");
            LocalDateTime localTimeInUnitedKingom = new LocalDateTime(2016, 06, 01, 10, 40);
            ZonedDateTime zonedStartDateTime = localTimeInUnitedKingom.InZoneStrictly(timeZoneUnitedKingdom);
            ZonedDateTime zonedFinishDateTime = zonedStartDateTime.Plus(Duration.FromMinutes(60));
            ZonedDateTime matchingUtcZonedDateTime = zonedStartDateTime.WithZone(DateTimeZone.Utc);

            var offsetStartTime = new OffsetDateTime(zonedStartDateTime.LocalDateTime, zonedStartDateTime.Offset);
            var offsetFinishTime = new OffsetDateTime(zonedFinishDateTime.LocalDateTime, zonedFinishDateTime.Offset);
            var offsetStartTimeUtc = matchingUtcZonedDateTime.ToOffsetDateTime();

            var testEvent = new OffsetDateTimeTestEntity
            {
                Description = "Can_Query_By_Equals_Using_A_UtcTime_OffsetDateTime_Stored_As_DateTimeOffset",
                SystemDateTimeOffset = DateTimeOffset.Now,
                StartOffsetDateTime = offsetStartTime,
                FinishOffsetDateTime = offsetFinishTime
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            // Even though two OffsetDateTimes may refer to the same instant...
            Assert.That(offsetStartTime.ToInstant(), Is.EqualTo(offsetStartTimeUtc.ToInstant()));
            // They are not considered equal by NodaTime unless the LocalDateTime and Offset parts are both equal.
            // (There is nothing wrong with this, it is perfectly valid logic for comparing two OffsetDateTimes.)
            Assert.That(offsetStartTime, Is.Not.EqualTo(offsetStartTimeUtc));

            // However we are storing OffsetDateTimes as DateTimeOffsets in the sql server.
            // So when using Linq and sending the expression to the sql server, 
            // the sql server will convert all DateTimeOffsets to utc time before comparing them.
            // Therefore the same two OffsetDateTimes that are not equal as shown above are seen as being equal by sql.
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<OffsetDateTimeTestEntity>().Where(x => x.StartOffsetDateTime == offsetStartTimeUtc);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }
        }


        /// <summary>
        /// Can we write and read a OffsetDateTime stored as a DateTimeOffset.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_OffsetDateTimeNullable_Stored_As_DateTimeOffset()
        {
            var testEvent = new OffsetDateTimeTestEntity
            {
                Description = "Can_Write_And_Read_OffsetDateTime_Stored_As_DateTimeOffset",
                SystemDateTimeOffset = DateTimeOffset.Now,
                StartOffsetDateTime = null,
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            OffsetDateTimeTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<OffsetDateTimeTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent, Is.EqualTo(testEvent));
            Assert.That(retrievedEvent.StartOffsetDateTime, Is.EqualTo(null));
        }
    }
}
