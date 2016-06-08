using Dematt.Airy.Core;
using NodaTime;

namespace Dematt.Airy.Tests.NodaTime.Entities
{
    public class ZonedDateTimeTestEntity : EntityWithIntId
    {
        public virtual string Description { get; set; }

        public virtual ZonedDateTime StartZonedDateTime { get; set; }

        public virtual ZonedDateTime? FinishZonedDateTime { get; set; }
    }
}
