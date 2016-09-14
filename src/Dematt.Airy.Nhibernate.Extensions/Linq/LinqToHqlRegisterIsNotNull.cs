using NHibernate.Linq.Functions;

namespace Dematt.Airy.Nhibernate.Extensions.Linq
{
    /// <summary>
    /// Registers the Linq to Hql Generators used to extend the NHibernate Linq Provider to help support the object.IsNotNull() extension method.
    /// </summary>    
    ///<remarks>
    /// <![CDATA[Use configuration.LinqToHqlGeneratorsRegistry<LinqToHqlRegisterIsNotNull>() to register this when building a SessionFactory.]]>
    /// </remarks>
    public class LinqToHqlRegisterIsNotNull : DefaultLinqToHqlGeneratorsRegistry
    {
        /// <summary>
        /// Constructor that registers the required Nhibernate Linq to Hql Generators for the Money type.
        /// </summary>
        public LinqToHqlRegisterIsNotNull()
        {
            this.Merge(new IsNotNullGenerator());
        }
    }
}
