using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaTime
{
    /// <summary>
    /// Runs once for the entire test assembly to build the database schema.
    /// </summary>
    [SetUpFixture]
    public class OneTimeSetup
    {
        /// <summary>
        /// Builds the database schema.
        /// </summary>
        [OneTimeSetUp]
        public void SetupOnceForAssembly()
        {
            string configFile = SessionFactoryProvider.GetFullPathForContentFile("hibernate.cfg.xml");
            var sessionFactoryProvider = new SessionFactoryProvider(configFile);
            sessionFactoryProvider.BuildSchema();
        }
    }
}
