using Dematt.Airy.Core;
using NodaTime;

namespace Dematt.Airy.Tests.NodaTime.Entities
{
    public class LocalDateTimeTestEntity : EntityWithIntId
    {
        public virtual string Description { get; set; }

        public virtual LocalDateTime StartLocalDateTime { get; set; }

        public virtual LocalDateTime? FinishLocalDateTime { get; set; }
    }
}
