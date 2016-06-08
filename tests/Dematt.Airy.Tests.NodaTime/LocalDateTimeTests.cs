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
    /// Tests for persisting the NodaTime  <see cref="LocalDateTime"/> type using the <see cref="LocalDateTimeType"/> NHibernate custom user type.
    /// </summary>
    public class LocalDateTimeTests : PersistenceTest
    {
        /// <summary>
        /// Can we write a LocalDateTime stored as a datetime2.
        /// </summary>
        [Test]
        public void Can_Write_LocalDateTime_Stored_As_DateTime2()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
                LocalDateTime startLocalDateTime = TestClock.Now.InZone(timeZone).LocalDateTime;
                var testEvent = new LocalDateTimeTestEntity
                {
                    Description = "Can_Write_LocalDateTime_Stored_As_DateTime2",
                    StartLocalDateTime = startLocalDateTime
                };
                session.Save(testEvent);
                transaction.Commit();
                Assert.That(testEvent.Id, Is.Not.Null);
            }
        }

        /// <summary>
        /// Can we write and read a LocalDateTime stored as a datetime2.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_LocalDateTime_Stored_As_DateTime2()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            LocalDateTime startLocalDateTime = TestClock.Now.InZone(timeZone).LocalDateTime;
            LocalDateTime finishLocalDateTime = startLocalDateTime.PlusHours(1);
            var testEvent = new LocalDateTimeTestEntity
            {
                Description = "Can_Write_And_Read_LocalDateTime_Stored_As_DateTime2",
                StartLocalDateTime = startLocalDateTime,
                FinishLocalDateTime = finishLocalDateTime
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            LocalDateTimeTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<LocalDateTimeTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent.StartLocalDateTime, Is.EqualTo(testEvent.StartLocalDateTime));
            Assert.That(retrievedEvent.FinishLocalDateTime, Is.EqualTo(testEvent.FinishLocalDateTime));
            Assert.That(retrievedEvent, Is.EqualTo(testEvent));
        }

        /// <summary>
        /// Can we write and read a null LocalDateTime stored as a datetime2.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_A_Null_LocalDateTime_Stored_As_DateTime2()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            LocalDateTime startLocalDateTime = TestClock.Now.InZone(timeZone).LocalDateTime;
            var testEvent = new LocalDateTimeTestEntity
            {
                Description = "Can_Write_And_Read_A_Null_LocalDateTime_Stored_As_DateTime2",
                StartLocalDateTime = startLocalDateTime,
                FinishLocalDateTime = null
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            LocalDateTimeTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<LocalDateTimeTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent.StartLocalDateTime, Is.EqualTo(testEvent.StartLocalDateTime));
            Assert.That(retrievedEvent.FinishLocalDateTime, Is.EqualTo(testEvent.FinishLocalDateTime));
        }

        /// <summary>
        /// Can we query for an LocalDateTime in the database using LINQ == logic.
        /// </summary>
        [Test]
        public void Can_Query_By_Equals_LocalDateTime_Stored_As_DateTime2()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            LocalDateTime startLocalDateTime = TestClock.Now.InZone(timeZone).LocalDateTime;
            LocalDateTime finishLocalDateTime = startLocalDateTime.PlusHours(1);
            var testEvent = new LocalDateTimeTestEntity
            {
                Description = "Can_Query_By_Equals_LocalDateTime_Stored_As_DateTime2",
                StartLocalDateTime = startLocalDateTime,
                FinishLocalDateTime = finishLocalDateTime
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
                var query = session.Query<LocalDateTimeTestEntity>().Where(x => x.StartLocalDateTime == startLocalDateTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<LocalDateTimeTestEntity>()
                    .Where(x => x.StartLocalDateTime == startLocalDateTime && x.FinishLocalDateTime == finishLocalDateTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }
        }

        /// <summary>
        /// Can we query for an LocalDateTime in the database using LINQ less than logic.
        /// </summary>
        [Test]
        public void Can_Query_By_LessThan_LocalDateTime_Stored_As_DateTime2()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            LocalDateTime startLocalDateTime = TestClock.Now.InZone(timeZone).LocalDateTime.Minus(Period.FromHours(1));
            LocalDateTime finishLocalDateTime = startLocalDateTime.PlusHours(3);
            var testEvent = new LocalDateTimeTestEntity
            {
                Description = "Can_Query_By_LessThan_LocalDateTime_Stored_As_DateTime2",
                StartLocalDateTime = startLocalDateTime,
                FinishLocalDateTime = finishLocalDateTime
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            LocalDateTime beforeLocalDateTime = startLocalDateTime.Plus(Period.FromHours(1));
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<LocalDateTimeTestEntity>()
                    .Where(x => x.Id == testEvent.Id && x.StartLocalDateTime < beforeLocalDateTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<LocalDateTimeTestEntity>()
                    .Where(x => x.Id == testEvent.Id && x.StartLocalDateTime < beforeLocalDateTime && x.FinishLocalDateTime <= finishLocalDateTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }
        }

        /// <summary>
        /// Can we query for an LocalDateTime in the database using LINQ greater than logic.
        /// </summary>
        [Test]
        public void Can_Query_By_GreaterThan_LocalDateTime_Stored_As_DateTime2()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            LocalDateTime startLocalDateTime = TestClock.Now.InZone(timeZone).LocalDateTime.Plus(Period.FromHours(1));
            LocalDateTime finishLocalDateTime = startLocalDateTime.PlusHours(3);
            var testEvent = new LocalDateTimeTestEntity
            {
                Description = "Can_Query_By_GreaterThan_LocalDateTime_Stored_As_DateTime2",
                StartLocalDateTime = startLocalDateTime,
                FinishLocalDateTime = finishLocalDateTime
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            LocalDateTime beforeLocalDateTime = startLocalDateTime.Minus(Period.FromHours(1));
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<LocalDateTimeTestEntity>()
                    .Where(x => x.Id == testEvent.Id && x.StartLocalDateTime > beforeLocalDateTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<LocalDateTimeTestEntity>()
                    .Where(x => x.Id == testEvent.Id && x.StartLocalDateTime > beforeLocalDateTime && x.FinishLocalDateTime >= finishLocalDateTime);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }
        }
    }
}
