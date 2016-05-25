using System.Linq;
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
    }
}
