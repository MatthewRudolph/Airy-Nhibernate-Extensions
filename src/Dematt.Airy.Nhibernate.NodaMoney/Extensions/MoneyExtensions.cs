using NodaMoney;

namespace Dematt.Airy.Nhibernate.NodaMoney.Extensions
{
    public static class MoneyExtensions
    {
        public static string ToCurrencyCode(this Money ext)
        {
            return ext.Currency.Code;
        }

        public static decimal ToAmount(this Money ext)
        {
            return ext.Amount;
        }
    }
}
