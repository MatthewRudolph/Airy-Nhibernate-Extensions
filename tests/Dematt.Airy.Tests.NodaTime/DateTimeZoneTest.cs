using System;
using System.Linq;
using Dematt.Airy.Tests.NodaTime.Entities;
using NHibernate;
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
        public void Can_We_Safley_Store_The_Longest_DateTimeZone_Id()
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
                    Description = "Can_Write_OffsetDateTime_Stored_As_DateTimeOffset",
                    StartDateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault()
                };
                session.Save(testEvent);
                transaction.Commit();
            }
        }
    }
}
