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
        /// Can we write a Money object stored as a Decimal for the amount and a string for the Currency code.
        /// </summary>
        [Test]
        public void Can_Write_Money_Stored_As_Decimal_And_String()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Money cost = new Money(2.99m, "GBP");
                Money retail = cost + new Money(0.01m, "GBP");
                var testMoney = new MoneyTestEntity
                {
                    Description = "Can_Write_Money_Stored_As_Decimal_And_String",
                    Cost = cost,
                    Retail = retail
                };
                session.Save(testMoney);
                transaction.Commit();
                Assert.That(testMoney.Id, Is.Not.Null);
            }
        }

        /// <summary>
        /// Can we write and read a Money object stored as a Decimal for the amount and a string for the Currency code.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_Money_Stored_As_Decimal_And_String()
        {
            MoneyTestEntity testMoney;
            MoneyTestEntity retrievedTestMoney;

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Money cost = new Money(3.00m, "GBP");
                Money retail = cost + new Money(0.02m, "GBP");
                testMoney = new MoneyTestEntity
                {
                    Description = "Can_Write_And_Read_Money_Stored_As_Decimal_And_String",
                    Cost = cost,
                    Retail = retail
                };
                session.Save(testMoney);
                transaction.Commit();
                Assert.That(testMoney.Id, Is.Not.Null);
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedTestMoney = session.Get<MoneyTestEntity>(testMoney.Id);
                transaction.Commit();
            }
            Assert.That(retrievedTestMoney, Is.Not.Null);
            Assert.That(retrievedTestMoney, Is.EqualTo(testMoney));
            Assert.That(retrievedTestMoney.Cost, Is.EqualTo(testMoney.Cost));
            Assert.That(retrievedTestMoney.Retail, Is.EqualTo(testMoney.Retail));
        }

        /// <summary>
        /// Can we write and read an Un-initialised Money object stored as a Decimal for the amount and a string for the Currency code.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_UnInitialised_Money_Stored_As_Decimal_And_String()
        {
            MoneyTestEntity testMoney;
            MoneyTestEntity retrievedTestMoney;

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                testMoney = new MoneyTestEntity
                {
                    Description = "Can_Write_And_Read_UnInitialised_Money_Stored_As_Decimal_And_String",
                };
                session.Save(testMoney);
                transaction.Commit();
                Assert.That(testMoney.Id, Is.Not.Null);
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedTestMoney = session.Get<MoneyTestEntity>(testMoney.Id);
                transaction.Commit();
            }
            Assert.That(retrievedTestMoney, Is.Not.Null);
            Assert.That(retrievedTestMoney, Is.EqualTo(testMoney));
            Assert.That(retrievedTestMoney.Cost, Is.EqualTo(testMoney.Cost));
            Assert.That(retrievedTestMoney.Retail, Is.EqualTo(testMoney.Retail));
        }

        /// <summary>
        /// Can we write and read a nullable Money object stored as a Decimal for the amount and a string for the Currency code.
        /// </summary>
        [Test]
        public void Can_Write_And_Read_Nullable_Money_Stored_As_Decimal_And_String()
        {
            MoneyTestEntity testMoney;
            MoneyTestEntity retrievedTestMoney;

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                testMoney = new MoneyTestEntity
                {
                    Description = "Can_Write_And_Read_Nullable_Money_Stored_As_Decimal_And_String",
                    Retail = null
                };
                session.Save(testMoney);
                transaction.Commit();
                Assert.That(testMoney.Id, Is.Not.Null);
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                retrievedTestMoney = session.Get<MoneyTestEntity>(testMoney.Id);
                transaction.Commit();
            }
            Assert.That(retrievedTestMoney, Is.Not.Null);
            Assert.That(retrievedTestMoney, Is.EqualTo(testMoney));
            Assert.That(retrievedTestMoney.Cost, Is.EqualTo(testMoney.Cost));
            Assert.That(retrievedTestMoney.Retail, Is.EqualTo(testMoney.Retail));
        }
    }
}
