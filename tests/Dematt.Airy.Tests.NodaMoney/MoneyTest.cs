using System.Collections.Generic;
using System.Linq;
using Dematt.Airy.Nhibernate.NodaMoney.Extensions;
using Dematt.Airy.Tests.NodaMoney.Entities;
using NHibernate;
using NHibernate.Linq;
using NodaMoney;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaMoney
{
    public class MoneyTest : PersistenceTest
    {
        /// <summary>
        /// Can we create an instance of the Noda Money struct.
        /// </summary>
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

        /// <summary>
        /// Can we get a list of currency supported by NodaMOney.
        /// </summary>
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

        /// <summary>
        /// Can we Query by equals a Money object stored as a Decimal for the amount and a string for the Currency code.
        /// </summary>
        [Test]
        public void Can_Query_By_Equals_Money_Stored_As_Decimal_And_String()
        {
            MoneyTestEntity testMoney;
            MoneyTestEntity retrievedTestMoney;

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Money cost = new Money(150.00m, "GBP");
                Money retail = cost + new Money(0.01m, "GBP");
                testMoney = new MoneyTestEntity
                {
                    Description = "Can_Query_By_Equals_Money_Stored_As_Decimal_And_String",
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
                var query = session.Query<MoneyTestEntity>().Where(x => x.Cost == new Money(150.00m, "GBP"));

                retrievedTestMoney = query.SingleOrDefault();
                transaction.Commit();
            }
            Assert.That(retrievedTestMoney, Is.Not.Null);
            Assert.That(retrievedTestMoney, Is.EqualTo(testMoney));
            if (retrievedTestMoney != null)
            {
                Assert.That(retrievedTestMoney.Cost, Is.EqualTo(testMoney.Cost));
                Assert.That(retrievedTestMoney.Retail, Is.EqualTo(testMoney.Retail));
            }
        }

        /// <summary>
        /// Can we Query by more than a Money object stored as a Decimal for the amount and a string for the Currency code.
        /// </summary>
        [Test]
        public void Can_Query_By_MoreThan_Money_Stored_As_Decimal_And_String()
        {
            MoneyTestEntity testMoney;
            MoneyTestEntity retrievedTestMoney;

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Money cost = new Money(800000000.00m, "GBP");
                Money retail = cost + new Money(0.01m, "GBP");
                testMoney = new MoneyTestEntity
                {
                    Description = "Can_Query_By_MoreThan_Money_Stored_As_Decimal_And_String",
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
                var query = session.Query<MoneyTestEntity>().Where(x => x.Retail.Value.Amount > 800000000.00m && x.Retail.Value.ToCurrencyCode() == "GBP");

                retrievedTestMoney = query.SingleOrDefault();
                transaction.Commit();
            }
            Assert.That(retrievedTestMoney, Is.Not.Null);
            Assert.That(retrievedTestMoney, Is.EqualTo(testMoney));
            if (retrievedTestMoney != null)
            {
                Assert.That(retrievedTestMoney.Cost, Is.EqualTo(testMoney.Cost));
                Assert.That(retrievedTestMoney.Retail, Is.EqualTo(testMoney.Retail));
            }
        }

        /// <summary>
        /// Can we Query by less than a Money object stored as a Decimal for the amount and a string for the Currency code.
        /// </summary>
        [Test]
        public void Can_Query_By_LessThan_Money_Stored_As_Decimal_And_String()
        {
            MoneyTestEntity testMoney;
            MoneyTestEntity retrievedTestMoney;

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Money cost = new Money(-800000000.00m, "USD");
                Money retail = cost + new Money(-0.01m, "USD");
                testMoney = new MoneyTestEntity
                {
                    Description = "Can_Query_By_LessThan_Money_Stored_As_Decimal_And_String",
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
                var query = session.Query<MoneyTestEntity>().Where(x => x.Retail.Value.Amount < -800000000.00m && x.Retail.Value.ToCurrencyCode() == "USD");

                retrievedTestMoney = query.SingleOrDefault();
                transaction.Commit();
            }
            Assert.That(retrievedTestMoney, Is.Not.Null);
            Assert.That(retrievedTestMoney, Is.EqualTo(testMoney));
            if (retrievedTestMoney != null)
            {
                Assert.That(retrievedTestMoney.Cost, Is.EqualTo(testMoney.Cost));
                Assert.That(retrievedTestMoney.Retail, Is.EqualTo(testMoney.Retail));
            }
        }

        /// <summary>
        /// Can we Query and aggregate using Sum a Money objects stored as a Decimal for the amount and a string for the Currency code.
        /// </summary>
        [Test]
        public void Can_Query_By_Sum_Money_Stored_As_Decimal_And_String()
        {
            decimal? total;

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Money cost = new Money(-800000000.00m, "USD");
                Money retail = cost + new Money(-0.01m, "USD");
                var testMoney = new MoneyTestEntity
                {
                    Description = "Can_Query_By_Sum_Money_Stored_As_Decimal_And_String",
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
                var query = session.Query<MoneyTestEntity>().Select(x => x.Cost).Sum(x => x.Amount);
                total = query;
                transaction.Commit();
            }
            Assert.That(total, Is.Not.Null);
        }

        /// <summary>
        /// Can we Query and aggregate using Sum with a filter a Money objects stored as a Decimal for the amount and a string for the Currency code.
        /// </summary>
        [Test]
        public void Can_Query_By_Sum_And_Filter_Money_Stored_As_Decimal_And_String()
        {
            decimal? total;

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                Money cost = new Money(-800000000.00m, "USD");
                Money retail = cost + new Money(-0.01m, "USD");
                var testMoney = new MoneyTestEntity
                {
                    Description = "Can_Query_By_Sum_And_Filter_Money_Stored_As_Decimal_And_String",
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
                var query = session.Query<MoneyTestEntity>()
                    .Where(x => x.Cost.ToCurrencyCode() == "GBP")
                    .Select(x => x.Cost)
                    .Sum(x => x.Amount);
                total = query;
                transaction.Commit();
            }
            Assert.That(total, Is.Not.Null);
        }
    }
}
