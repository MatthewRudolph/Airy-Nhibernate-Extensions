

using NHibernate.Linq.Functions;

namespace Dematt.Airy.Nhibernate.NodaTime.Helpers
{
    // Use configuration.LinqToHqlGeneratorsRegistry<LinqToHqlGeneratorsRegistry>() to register this
    public class LinqToHqlGeneratorsRegistry : DefaultLinqToHqlGeneratorsRegistry
    {
        public LinqToHqlGeneratorsRegistry()
        {
            this.Merge(new ZonedDateTimeGtLtGenerator());
        }
    }
}
