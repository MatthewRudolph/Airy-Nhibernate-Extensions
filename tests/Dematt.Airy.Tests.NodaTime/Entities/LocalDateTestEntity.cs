using Dematt.Airy.Core;
using NodaTime;

namespace Dematt.Airy.Tests.NodaTime.Entities
{
    public class LocalDateTestEntity : EntityWithIntId
    {
        public virtual string Description { get; set; }

        public virtual LocalDate StartLocalDate { get; set; }

        public virtual LocalDate? FinishLocalDate { get; set; }
    }
}
