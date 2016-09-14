using NHibernate.Linq.Functions;

namespace Dematt.Airy.Nhibernate.NodaMoney.Helpers
{
    /// <summary>
    /// Registers the Linq to Hql Generators used to extend the NHibernate Linq Provider to help support improved Linq querying of the Money type.
    /// </summary>    
    ///<remarks>
    /// <![CDATA[Use configuration.LinqToHqlGeneratorsRegistry<LinqToHqlRegisterMoney>() to register this when building a SessionFactory.]]>
    /// </remarks>
    public class LinqToHqlRegisterMoney : DefaultLinqToHqlGeneratorsRegistry
    {
        /// <summary>
        /// Constructor that registers the required Nhibernate Linq to Hql Generators for the NodaMoney type.
        /// </summary>
        public LinqToHqlRegisterMoney()
        {
            this.Merge(new MoneyAmountHqlGenerator());
            this.Merge(new MoneyToAmountHqlGenerator());
            this.Merge(new MoneyToCurrencyCodeHqlGenerator());
        }
    }
}
