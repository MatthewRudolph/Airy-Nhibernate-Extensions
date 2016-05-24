using System;
using Dematt.Airy.Tests.NodaTime.Entities;
using NHibernate;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaTime
{
    public class NhibernateNodaTimePersistenceTests
    {
        private static SessionFactoryProvider _sessionFactoryProvider;

        private static ISessionFactory _sessionFactory;

        private ISession _session;

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

        /// <summary>
        /// Sets up the required objects and starts a transaction before each test.
        /// </summary>        
        [SetUp]
        public void BeforeEachTest()
        {
            // Create the user service and it's dependencies.
            _session = _sessionFactory.OpenSession();
            _session.BeginTransaction();
        }

        /// <summary>
        /// Commits the transactions after each test.
        /// </summary>
        [TearDown]
        public void AfterEachTest()
        {
            _session.Transaction.Commit();
        }

        [Test]
        public void CanSaveANewTestEvent()
        {
            var testEvent = new TestEvent
            {
                Description = "CanSaveANewTestEvent",
                SystemDateTimeOffset = DateTimeOffset.Now
            };
            _session.Save(testEvent);
        }
    }
}
