using System.Collections.Generic;
using Dematt.Airy.Tests.NodaMoney.Entities;
using NHibernate;
using NodaMoney;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaMoney
{
    public class MoneyTest : PersistenceTest
    {
        [Test]
        public void Can_Create_Instances_Of_Money()
        {
            Money localMoney = new Money(1.99m);
            Money gbpMoney = new Money(1.99m, "GBP");
            Money fullMoney = new Money(1.99m, Currency.FromCode("GBP", "ISO-4217"));

            Assert.That(localMoney, Is.Not.Null);
            Assert.That(gbpMoney, Is.Not.Null);
            Assert.That(fullMoney, Is.Not.Null);
        }

        [Test]
        public void Can_Get_A_List_Of_Currencies()
        {
            IEnumerable<Currency> allCurrencies = Currency.GetAllCurrencies();

            Assert.That(allCurrencies, Is.Not.Null);
        }

        /// <summary>
        /// Can we write a Money object stored as a Decimal and a string for the Currency code.
        /// </summary>
        [Test]
        public void Can_Write_Money_Stored_As_Decimal_And_String()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Money cost = new Money(2.99m, "GBP");

                var testMoney = new MoneyTestEntity()
                {
                    Description = "Can_Write_Money_Stored_As_Decimal_And_String",
                    Cost = cost
                };
                session.Save(testMoney);
                transaction.Commit();

                Assert.That(testMoney.Id, Is.Not.Null);
            }
        }
    }
}
