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
    /// Tests for persisting the NodaTime  <see cref="Instant"/> type using the <see cref="InstantType"/> NHibernate custom user type.
    /// </summary>
    public class InstantTests : PersistenceTest
    {
        /// <summary>
        /// Can we write a Instant stored as an Int64.
        /// </summary>
        [Test]
        public void Can_Write_Instant_Stored_As_Int64()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Instant startInstant = SystemClock.Instance.Now;
                var testEvent = new InstantTestEntity
                {
                    Description = "Can_Write_Instant_Stored_As_Int64",
                    StartInstant = startInstant,
                    FinishInstant = startInstant.Plus(Duration.FromHours(1))
                };
                session.Save(testEvent);
                transaction.Commit();
                Assert.That(testEvent.Id, Is.Not.Null);
            }
        }

        /// <summary>
        /// Can we write and read an Instant stored as a Int64.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_Instant_Stored_As_Int64()
        {
            Instant startInstant = SystemClock.Instance.Now;
            var testEvent = new InstantTestEntity
            {
                Description = "Can_Write_And_Read_Instant_Stored_As_Int64",
                StartInstant = startInstant,
                FinishInstant = startInstant.Plus(Duration.FromHours(1))
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            InstantTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<InstantTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent.StartInstant, Is.EqualTo(testEvent.StartInstant));
            Assert.That(retrievedEvent.FinishInstant, Is.EqualTo(testEvent.FinishInstant));
            Assert.That(retrievedEvent, Is.EqualTo(testEvent));
        }

        /// <summary>
        /// Can we write and read an null Instant stored as a Int64.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_A_Null_Instant_Stored_As_Int64()
        {
            Instant startInstant = SystemClock.Instance.Now;
            var testEvent = new InstantTestEntity
            {
                Description = "Can_Write_And_Read_A_Null_Instant_Stored_As_Int64",
                StartInstant = startInstant,
                FinishInstant = null
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            InstantTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<InstantTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent.StartInstant, Is.EqualTo(testEvent.StartInstant));
            Assert.That(retrievedEvent.FinishInstant, Is.EqualTo(testEvent.FinishInstant));
            Assert.That(retrievedEvent, Is.EqualTo(testEvent));
        }

        /// <summary>
        /// Can we query for an Instant in the database using LINQ == logic.
        /// </summary>
        [Test]
        public void Can_Query_By_Equals_Instant_Stored_As_Int64()
        {

            Instant startInstant = SystemClock.Instance.Now;
            Instant finishInstant = startInstant.Plus(Duration.FromHours(1));
            var testEvent = new InstantTestEntity
            {
                Description = "Can_Query_By_Equals_Instant_Stored_As_Int64",
                StartInstant = startInstant,
                FinishInstant = finishInstant
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
                var query = session.Query<InstantTestEntity>().Where(x => x.StartInstant == startInstant);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<InstantTestEntity>()
                    .Where(x => x.StartInstant == startInstant && x.FinishInstant == finishInstant);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }
        }
    }
}
