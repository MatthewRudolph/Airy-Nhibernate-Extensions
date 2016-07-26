using System.Collections.Generic;
using NodaMoney;
using NUnit.Framework;

namespace Dematt.Airy.Tests.NodaMoney
{
    public class MoneyTest
    {
        [Test]
        public void Can_Create_An_Instance_Of_Money()
        {
            Money localMoney = new Money(1.99m);
            Money gbpMoney = new Money(1.99m, "GBP");
            Money fullMoney = new Money(1.99m, Currency.FromCode("GBP", "ISO-4217"));
        }

        [Test]
        public void Can_Get_A_List_Of_Currencies()
        {
            IEnumerable<Currency> allCurrencies = Currency.GetAllCurrencies();
            Assert.That(allCurrencies, Is.Not.Null);
        }
    }
}
