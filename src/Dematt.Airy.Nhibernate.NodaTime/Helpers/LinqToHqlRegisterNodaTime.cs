using NHibernate.Linq.Functions;

namespace Dematt.Airy.Nhibernate.NodaTime.Helpers
{
    /// <summary>
    /// Registers the Linq to Hql Generators used to extend the NHibernate Linq Provider to help support the ZonedDateTime.ToDateTimeOffset() extension method.
    /// </summary>    
    ///<remarks>
    /// <![CDATA[Use configuration.LinqToHqlGeneratorsRegistry<LinqToHqlRegisterNodaTime>() to register this when building a SessionFactory.]]>
    /// </remarks>    
    public class LinqToHqlRegisterNodaTime : DefaultLinqToHqlGeneratorsRegistry
    {
        /// <summary>
        /// Constructor that registers the required Nhibernate Linq to Hql Generators for the NodaTime types.
        /// </summary>
        public LinqToHqlRegisterNodaTime()
        {
            this.Merge(new ZonedDateTimeGtLtGenerator());
        }
    }
}
