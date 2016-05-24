using System;
using System.Linq;
using Dematt.Airy.Tests.NodaTime.Entities;
using NHibernate;
using NHibernate.Linq;
using NodaTime;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaTime
{
    public class NhibernateNodaTimePersistenceTests
    {
        private static SessionFactoryProvider _sessionFactoryProvider;

        private static ISessionFactory _sessionFactory;

        /// <summary>
        /// Creates an NHibernate Session factory and builds a fresh database schema.
        /// </summary>
        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            string configFile = SessionFactoryProvider.GetFullPathForContentFile("hibernate.cfg.xml");
            _sessionFactoryProvider = new SessionFactoryProvider(configFile);
            _sessionFactory = _sessionFactoryProvider.DefaultSessionFactory;
            _sessionFactoryProvider.BuildSchema();
        }

        /// <summary>
        /// Disposes of the objects once the test has completed.
        /// </summary>
        [OneTimeTearDown]
        public void AfterAllTests()
        {
            _sessionFactory.Dispose();
            _sessionFactory = null;
            _sessionFactoryProvider = null;
        }

        [Test]
        public void CanPersiste_NodaTime_OffsetDateTime()
        {
            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
                Instant now = SystemClock.Instance.Now;
                ZonedDateTime zonedNowDateTime = now.InZone(timeZone);


                var testEvent = new TestEvent
                {
                    Description = "CanPersiste_NodaTime_OffsetDateTime",
                    SystemDateTimeOffset = DateTimeOffset.Now,
                    FinishOffsetDateTime = new OffsetDateTime(zonedNowDateTime.LocalDateTime, zonedNowDateTime.Offset)
                };
                session.Save(testEvent);
                transaction.Commit();
            }
        }

        [Test]
        public void CanPersiste_NodaTime_CalculatedOffsetDateTime()
        {
            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
                Instant now = SystemClock.Instance.Now;
                ZonedDateTime zonedNowDateTime = now.InZone(timeZone);
                var zonedFinishDateTime = zonedNowDateTime.Plus(Duration.FromMinutes(60));

                var testEvent = new TestEvent
                {
                    Description = "CanPersiste_NodaTime_CalculatedOffsetDateTime",
                    SystemDateTimeOffset = DateTimeOffset.Now,
                    StartOffsetDateTime = new OffsetDateTime(zonedNowDateTime.LocalDateTime, zonedNowDateTime.Offset),
                    FinishOffsetDateTime =
                        new OffsetDateTime(zonedFinishDateTime.LocalDateTime, zonedFinishDateTime.Offset)
                };
                session.Save(testEvent);
                transaction.Commit();
            }
        }


        [Test]
        public void CanQuery_NodaTime_OffsetDateTimeEquals()
        {
            var timeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            Instant now = SystemClock.Instance.Now;
            ZonedDateTime zonedNowDateTime = now.InZone(timeZone);

            var zonedFinishDateTime = zonedNowDateTime.Plus(Duration.FromMinutes(60));
            var offsetFinishTime = new OffsetDateTime(zonedFinishDateTime.LocalDateTime, zonedFinishDateTime.Offset);

            var testEvent = new TestEvent
                {
                    Description = "CanPersiste_NodaTime_CalculatedOffsetDateTime",
                    SystemDateTimeOffset = DateTimeOffset.Now,
                    StartOffsetDateTime = new OffsetDateTime(zonedNowDateTime.LocalDateTime, zonedNowDateTime.Offset),
                    FinishOffsetDateTime = offsetFinishTime
                };

            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                session.Save(testEvent);
                transaction.Commit();
            }

            using (ISession session = _sessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<TestEvent>().Where(x => x.FinishOffsetDateTime == offsetFinishTime);
                var retrievedEvent = query.SingleOrDefault();
                Assert.That(testEvent.Id, Is.EqualTo(retrievedEvent.Id));
                transaction.Commit();
            }
        }
    }
}
