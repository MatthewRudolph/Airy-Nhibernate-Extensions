using NodaMoney;

namespace Dematt.Airy.Nhibernate.NodaMoney.Extensions
{
    /// <summary>
    /// Extensions to the Money struct that are used to help support improved Linq querying.
    /// </summary>
    public static class MoneyExtensions
    {
        /// <summary>
        /// Returns the Code from the <see cref="Currency"/> of the money.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the Currencies Code</returns>
        public static string ToCurrencyCode(this Money ext)
        {
            return ext.Currency.Code;
        }

        /// <summary>
        /// Returns the Amount from the money.
        /// </summary>
        /// <returns>A <see cref="decimal"/> that represents the amount.</returns>
        public static decimal ToAmount(this Money ext)
        {
            return ext.Amount;
        }
    }
}
