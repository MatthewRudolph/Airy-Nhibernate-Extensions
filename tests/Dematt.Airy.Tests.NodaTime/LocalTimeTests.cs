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
    /// Tests for persisting the NodaTime  <see cref="LocalTime"/> type using the <see cref="LocalTimeType"/> NHibernate custom user type.
    /// </summary>
    public class LocalTimeTests : PersistenceTest
    {
        /// <summary>
        /// Can we write a LocalTime stored as a time.
        /// </summary>
        [Test]
        public void Can_Write_LocalTime_Stored_As_Time()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
                LocalTime startLocalTime = TestClock.GetCurrentInstant().InZone(timeZone).TimeOfDay;
                var testEvent = new LocalTimeTestEntity
                {
                    Description = "Can_Write_LocalTime_Stored_As_Time",
                    StartLocalTime = startLocalTime
                };
                session.Save(testEvent);
                transaction.Commit();
                Assert.That(testEvent.Id, Is.Not.Null);
            }
        }

        /// <summary>
        /// Can we write and read a LocalTime stored as a time.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_LocalTime_Stored_As_Time()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            LocalTime startLocalTime = TestClock.GetCurrentInstant().InZone(timeZone).TimeOfDay;
            LocalTime finishLocalTime = startLocalTime.PlusHours(1);
            var testEvent = new LocalTimeTestEntity
            {
                Description = "Can_Write_And_Read_LocalTime_Stored_As_Time",
                StartLocalTime = startLocalTime,
                FinishLocalTime = finishLocalTime
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            LocalTimeTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<LocalTimeTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent.StartLocalTime, Is.EqualTo(testEvent.StartLocalTime));
            Assert.That(retrievedEvent.FinishLocalTime, Is.EqualTo(testEvent.FinishLocalTime));
            Assert.That(retrievedEvent, Is.EqualTo(testEvent));
        }

        /// <summary>
        /// Can we write and read a null LocalTime stored as a time.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_A_Null_LocalTime_Stored_As_Time()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            LocalTime startLocalTime = TestClock.GetCurrentInstant().InZone(timeZone).TimeOfDay;
            var testEvent = new LocalTimeTestEntity
            {
                Description = "Can_Write_And_Read_A_Null_LocalTime_Stored_As_Time",
                StartLocalTime = startLocalTime,
                FinishLocalTime = null
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            LocalTimeTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<LocalTimeTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent.StartLocalTime, Is.EqualTo(testEvent.StartLocalTime));
            Assert.That(retrievedEvent.FinishLocalTime, Is.EqualTo(testEvent.FinishLocalTime));
        }

        /// <summary>
        /// Can we query for an LocalTime in the database using LINQ == logic.
        /// </summary>
        [Test]
        public void Can_Query_By_Equals_LocalTime_Stored_As_Time()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            LocalTime startLocalTime = TestClock.GetCurrentInstant().InZone(timeZone).TimeOfDay;
            LocalTime finishLocalTime = startLocalTime.PlusHours(1);
            var testEvent = new LocalTimeTestEntity
            {
                Description = "Can_Query_By_Equals_LocalTime_Stored_As_Time",
                StartLocalTime = startLocalTime,
                FinishLocalTime = finishLocalTime
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
                var query = session.Query<LocalTimeTestEntity>().Where(x => x.StartLocalTime == startLocalTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<LocalTimeTestEntity>()
                    .Where(x => x.StartLocalTime == startLocalTime && x.FinishLocalTime == finishLocalTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }
        }

        /// <summary>
        /// Can we query for an LocalTime in the database using LINQ less than logic.
        /// </summary>
        [Test]
        public void Can_Query_By_LessThan_LocalTime_Stored_As_Time()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            LocalTime startLocalTime = TestClock.GetCurrentInstant().InZone(timeZone).TimeOfDay.Minus(Period.FromHours(1));
            LocalTime finishLocalTime = startLocalTime.PlusHours(3);
            var testEvent = new LocalTimeTestEntity
            {
                Description = "Can_Query_By_LessThan_LocalTime_Stored_As_Time",
                StartLocalTime = startLocalTime,
                FinishLocalTime = finishLocalTime
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            LocalTime beforeLocalTime = startLocalTime.Plus(Period.FromHours(1));
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<LocalTimeTestEntity>()
                    .Where(x => x.Id == testEvent.Id && x.StartLocalTime < beforeLocalTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<LocalTimeTestEntity>()
                    .Where(x => x.Id == testEvent.Id && x.StartLocalTime < beforeLocalTime && x.FinishLocalTime <= finishLocalTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }
        }

        /// <summary>
        /// Can we query for an LocalTime in the database using LINQ greater than logic.
        /// </summary>
        [Test]
        public void Can_Query_By_GreaterThan_LocalTime_Stored_As_Time()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            LocalTime startLocalTime = TestClock.GetCurrentInstant().InZone(timeZone).TimeOfDay.Plus(Period.FromHours(1));
            LocalTime finishLocalTime = startLocalTime.PlusHours(3);
            var testEvent = new LocalTimeTestEntity
            {
                Description = "Can_Query_By_GreaterThan_LocalTime_Stored_As_Time",
                StartLocalTime = startLocalTime,
                FinishLocalTime = finishLocalTime
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            LocalTime beforeLocalTime = startLocalTime.Minus(Period.FromHours(1));
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<LocalTimeTestEntity>()
                    .Where(x => x.Id == testEvent.Id && x.StartLocalTime > beforeLocalTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<LocalTimeTestEntity>()
                    .Where(x => x.Id == testEvent.Id && x.StartLocalTime > beforeLocalTime && x.FinishLocalTime >= finishLocalTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }
        }
    }
}
