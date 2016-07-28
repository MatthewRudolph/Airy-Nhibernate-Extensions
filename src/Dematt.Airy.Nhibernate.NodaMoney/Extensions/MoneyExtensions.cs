using NodaMoney;

namespace Dematt.Airy.Nhibernate.NodaMoney.Extensions
{
    public static class MoneyExtensions
    {
        public static string GetCurrencyCode(this Money ext)
        {
            return ext.Currency.Code;
        }
    }
}
