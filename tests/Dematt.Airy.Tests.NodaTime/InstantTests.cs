using System;
using Dematt.Airy.Tests.NodaTime.Entities;
using NHibernate;
using NodaTime;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaTime
{
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
                Instant now = SystemClock.Instance.Now;
                var testEvent = new InstantTestEntity
                {
                    Description = "Can_Write_Instant_Stored_As_Int64",
                    StartInstant = now,
                    FinishInstant = now.Plus(Duration.FromHours(1))
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
        public void Can_Write_And_Read_Instant_Stored_As_Int64()
        {
            Instant now = SystemClock.Instance.Now;
            var testEvent = new InstantTestEntity
            {
                Description = "Can_Write_Instant_Stored_As_Int64",
                StartInstant = now,
                FinishInstant = now.Plus(Duration.FromHours(1))
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
    }
}
