using NHibernate;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaTime
{
    public abstract class PersistenceTest
    {
        protected static SessionFactoryProvider SessionFactoryProvider;
        protected static ISessionFactory SessionFactory;

        /// <summary>
        /// Creates an NHibernate Session factory and builds a fresh database schema.
        /// </summary>
        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            string configFile = SessionFactoryProvider.GetFullPathForContentFile("hibernate.cfg.xml");
            SessionFactoryProvider = new SessionFactoryProvider(configFile);
            SessionFactory = SessionFactoryProvider.DefaultSessionFactory;
            SessionFactoryProvider.BuildSchema();
        }

        /// <summary>
        /// Disposes of the objects once the test has completed.
        /// </summary>
        [OneTimeTearDown]
        public void AfterAllTests()
        {
            SessionFactory.Dispose();
            SessionFactory = null;
            SessionFactoryProvider = null;
        }
    }
}