using NHibernate;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaMoney
{
    /// <summary>
    /// Base abstract class for persistence tests.
    /// Creates a NHibernate session factory that sessions can be obtained from.
    /// </summary>
    public abstract class PersistenceTest
    {
        protected static SessionFactoryProvider SessionFactoryProvider;
        protected static ISessionFactory SessionFactory;

        /// <summary>
        /// Creates an NHibernate Session factory.
        /// </summary>
        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            string configFile = SessionFactoryProvider.GetFullPathForContentFile("hibernate.cfg.xml");
            SessionFactoryProvider = new SessionFactoryProvider(configFile);
            SessionFactory = SessionFactoryProvider.DefaultSessionFactory;
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