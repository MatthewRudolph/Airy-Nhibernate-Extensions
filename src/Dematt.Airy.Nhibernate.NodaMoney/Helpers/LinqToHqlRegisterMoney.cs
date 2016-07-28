using NHibernate.Linq.Functions;

namespace Dematt.Airy.Nhibernate.NodaMoney.Helpers
{
    // Use configuration.LinqToHqlGeneratorsRegistry<LinqToHqlGeneratorsRegistry>() to register this
    public class LinqToHqlRegisterMoney : DefaultLinqToHqlGeneratorsRegistry
    {
        public LinqToHqlRegisterMoney()
        {
            this.Merge(new MoneyAmountHqlGenerator());
            this.Merge(new MoneyCurrencyHqlGenerator());
            this.Merge(new MoneyCurrencyPropertyHqlGenerator());
        }
    }
}
