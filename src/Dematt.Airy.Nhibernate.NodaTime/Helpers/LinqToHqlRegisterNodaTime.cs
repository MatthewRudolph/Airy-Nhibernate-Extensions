

using NHibernate.Linq.Functions;

namespace Dematt.Airy.Nhibernate.NodaTime.Helpers
{
    // Use configuration.LinqToHqlGeneratorsRegistry<LinqToHqlRegisterNodaTime>() to register this
    public class LinqToHqlRegisterNodaTime : DefaultLinqToHqlGeneratorsRegistry
    {
        public LinqToHqlRegisterNodaTime()
        {
            this.Merge(new ZonedDateTimeGtLtGenerator());
        }
    }
}
