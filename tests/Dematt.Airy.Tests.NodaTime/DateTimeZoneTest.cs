using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Dematt.Airy.Tests.NodaTime.Entities;
using NHibernate;
using NHibernate.Linq;
using NodaTime;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaTime
{
    public class DateTimeZoneTest : PersistenceTest
    {
        /// <summary>
        /// Is the DateTimeZone field size large enough to store the longest DateTimeZone id.
        /// </summary>
        [Test]
        public void Can_Safley_Store_The_Longest_DateTimeZone_Id()
        {
            const int idFieldSize = 35;
            var tzdbTimeZones = DateTimeZoneProviders.Tzdb.Ids;
            var longestTzdb = tzdbTimeZones.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);
            var bclTimeZones = DateTimeZoneProviders.Bcl.Ids;
            var longestBcl = bclTimeZones.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur);
            Assert.That(longestTzdb.Length, Is.LessThan(idFieldSize));
            Assert.That(longestBcl.Length, Is.LessThan(idFieldSize));
        }

        /// <summary>
        /// Can we write a DateTimeZone stored as a string.
        /// </summary>
        [Test]
        public void Can_Write_DateTimeZone_Stored_As_String()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var testEvent = new DateTimeZoneTestEntity
                {
                    Description = "Can_Write_DateTimeZone_Stored_As_String",
                    StartDateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault(),
                    FinishDateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault()
                };
                session.Save(testEvent);
                transaction.Commit();
            }
        }

        /// <summary>
        /// Can we write and read a OffsetDateTime stored as a DateTimeOffset.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_DateTimeZone_Stored_As_String()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Australia/West");
            var testEvent = new DateTimeZoneTestEntity
            {
                Description = "Can_Write_And_Read_DateTimeZone_Stored_As_String",
                StartDateTimeZone = timeZone,
                FinishDateTimeZone = timeZone
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            DateTimeZoneTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<DateTimeZoneTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent, Is.EqualTo(testEvent));
        }

        [Test]
        [SuppressMessage("ReSharper", "PossibleUnintendedReferenceComparison")]
        public void Can_Query_By_Equals_DateTimeZone_Stored_As_String()
        {
            var systemTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            var spainTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Madrid");

            var testEvent = new DateTimeZoneTestEntity
            {
                Description = "Can_Query_By_Equals_DateTimeZone_Stored_As_String",
                StartDateTimeZone = systemTimeZone,
                FinishDateTimeZone = spainTimeZone
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
                var query = session.Query<DateTimeZoneTestEntity>().Where(x => x.StartDateTimeZone == systemTimeZone);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var macthTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Madrid");
                var query = session.Query<DateTimeZoneTestEntity>().Where(x => x.FinishDateTimeZone == macthTimeZone);
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
        public void Can_Write_And_Read_Null_DateTimeZone_Stored_As_String()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Australia/West");
            var testEvent = new DateTimeZoneTestEntity
            {
                Description = "Can_Write_And_Read_Null_DateTimeZone_Stored_As_String",
                StartDateTimeZone = timeZone,
                FinishDateTimeZone = null
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            DateTimeZoneTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<DateTimeZoneTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent, Is.EqualTo(testEvent));
        }
    }
}
