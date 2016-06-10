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
    /// Tests for persisting the NodaTime  <see cref="Offset"/> type using the <see cref="OffsetType"/> NHibernate custom user type.
    /// </summary>
    public class OffsetTests : PersistenceTest
    {
        /// <summary>
        /// Can we write a Offset stored as a time.
        /// </summary>
        [Test]
        public void Can_Write_Offset_Stored_As_Time()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Offset startOffset = Offset.FromHours(1);
                var testEvent = new OffsetTestEntity
                {
                    Description = "Can_Write_Offset_Stored_As_Time",
                    StartOffset = startOffset
                };
                session.Save(testEvent);
                transaction.Commit();
                Assert.That(testEvent.Id, Is.Not.Null);
            }
        }

        /// <summary>
        /// Can we write and read a Offset stored as a time.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_Offset_Stored_As_Time()
        {
            Offset startOffset = Offset.FromHoursAndMinutes(1, 30);
            Offset finishOffset = Offset.FromHours(-1);
            var testEvent = new OffsetTestEntity
            {
                Description = "Can_Write_And_Read_Offset_Stored_As_Time",
                StartOffset = startOffset,
                FinishOffset = finishOffset
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            OffsetTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<OffsetTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent.StartOffset, Is.EqualTo(testEvent.StartOffset));
            Assert.That(retrievedEvent.FinishOffset, Is.EqualTo(testEvent.FinishOffset));
            Assert.That(retrievedEvent, Is.EqualTo(testEvent));
        }

        /// <summary>
        /// Can we write and read a null Offset stored as a time.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_A_Null_Offset_Stored_As_Time()
        {

            Offset startOffset = Offset.FromHoursAndMinutes(-1, -30);
            var testEvent = new OffsetTestEntity
            {
                Description = "Can_Write_And_Read_A_Null_Offset_Stored_As_Time",
                StartOffset = startOffset,
                FinishOffset = null
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            OffsetTestEntity retrievedEvent;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedEvent = session.Get<OffsetTestEntity>(testEvent.Id);
                transaction.Commit();
            }

            Assert.That(retrievedEvent.StartOffset, Is.EqualTo(testEvent.StartOffset));
            Assert.That(retrievedEvent.FinishOffset, Is.EqualTo(testEvent.FinishOffset));
        }

        /// <summary>
        /// Can we query for an Offset in the database using LINQ == logic.
        /// </summary>
        [Test]
        public void Can_Query_By_Equals_Offset_Stored_As_Time()
        {
            Offset startOffset = Offset.FromHoursAndMinutes(1, 30);
            Offset finishOffset = Offset.FromHours(-1);
            var testEvent = new OffsetTestEntity
            {
                Description = "Can_Query_By_Equals_Offset_Stored_As_Time",
                StartOffset = startOffset,
                FinishOffset = finishOffset
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
                var query = session.Query<OffsetTestEntity>().Where(x => x.StartOffset == startOffset);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<OffsetTestEntity>()
                    .Where(x => x.StartOffset == startOffset && x.FinishOffset == finishOffset);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }
        }

        /// <summary>
        /// Can we query for an Offset in the database using LINQ less than logic.
        /// </summary>
        [Test]
        public void Can_Query_By_LessThan_Offset_Stored_As_Time()
        {
            Offset startOffset = Offset.FromHoursAndMinutes(0, 15);
            Offset finishOffset = Offset.FromHours(-3);
            var testEvent = new OffsetTestEntity
            {
                Description = "Can_Query_By_LessThan_Offset_Stored_As_Time",
                StartOffset = startOffset,
                FinishOffset = finishOffset
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            Offset beforeOffset = Offset.FromHoursAndMinutes(0, 30);
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<OffsetTestEntity>()
                    .Where(x => x.Id == testEvent.Id && x.StartOffset < beforeOffset);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<OffsetTestEntity>()
                    .Where(x => x.Id == testEvent.Id && x.StartOffset < beforeOffset && x.FinishOffset <= finishOffset);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }
        }

        /// <summary>
        /// Can we query for an Offset in the database using LINQ greater than logic.
        /// </summary>
        [Test]
        public void Can_Query_By_GreaterThan_Offset_Stored_As_Time()
        {
            Offset startOffset = Offset.FromHoursAndMinutes(11, 0);
            Offset finishOffset = Offset.FromHours(12);
            var testEvent = new OffsetTestEntity
            {
                Description = "Can_Query_By_GreaterThan_Offset_Stored_As_Time",
                StartOffset = startOffset,
                FinishOffset = finishOffset
            };

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            Offset beforeOffset = Offset.FromHoursAndMinutes(10, 30);
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<OffsetTestEntity>()
                    .Where(x => x.Id == testEvent.Id && x.StartOffset > beforeOffset);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<OffsetTestEntity>()
                    .Where(x => x.Id == testEvent.Id && x.StartOffset > beforeOffset && x.FinishOffset >= finishOffset);
                var retrievedEvent = query.SingleOrDefault();
                transaction.Commit();
                Assert.That(testEvent, Is.Not.Null);
                Assert.That(testEvent, Is.EqualTo(retrievedEvent));
            }
        }
    }
}
