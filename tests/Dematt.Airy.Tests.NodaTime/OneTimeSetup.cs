using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaTime
{
    [SetUpFixture]
    public class OneTimeSetup
    {
        [OneTimeSetUp]
        public void SetupOnceForAssembly()
        {
            string configFile = SessionFactoryProvider.GetFullPathForContentFile("hibernate.cfg.xml");
            var sessionFactoryProvider = new SessionFactoryProvider(configFile);
            sessionFactoryProvider.BuildSchema();
        }
    }
}
