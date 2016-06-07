using Dematt.Airy.Nhibernate.NodaTime;
using Dematt.Airy.Tests.NodaTime.Entities;
using NHibernate;
using NodaTime;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaTime
{
    /// <summary>
    /// Tests for persisting the NodaTime  <see cref="LocalDate"/> type using the <see cref="LocalDateType"/> NHibernate custom user type.
    /// </summary>
    public class LocalDateTests : PersistenceTest
    {
        /// <summary>
        /// Can we write a LocalDate stored as a date.
        /// </summary>
        [Test]
        public void Can_Write_LocalDate_Stored_As_Date()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
                LocalDate startLocalDate = TestClock.Now.InZone(timeZone).Date;
                var testEvent = new LocalDateTestEntity
                {
                    Description = "Can_Write_LocalDate_Stored_As_Date",
                    StartLocalDate = startLocalDate
                };
                session.Save(testEvent);
                transaction.Commit();
                Assert.That(testEvent.Id, Is.Not.Null);
            }
        }

        /// <summary>
        /// Can we write and read a LocalDate stored as a date.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_LocalDate_Stored_As_Date()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            LocalDate startLocalDate = TestClock.Now.InZone(timeZone).Date;
            LocalDate finishLocalDate = startLocalDate.PlusDays(1);
            var testEvent = new LocalDateTestEntity
            {
                Description = "Can_Write_And_Read_LocalDate_Stored_As_Date",
                StartLocalDate = startLocalDate,
                FinishLocalDate = finishLocalDate
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            LocalDateTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<LocalDateTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent.StartLocalDate, Is.EqualTo(testEvent.StartLocalDate));
            Assert.That(retrievedEvent.FinishLocalDate, Is.EqualTo(testEvent.FinishLocalDate));
            Assert.That(retrievedEvent, Is.EqualTo(testEvent));
        }
    }
}
