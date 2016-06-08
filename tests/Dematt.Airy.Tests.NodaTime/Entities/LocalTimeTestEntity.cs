using Dematt.Airy.Core;
using NodaTime;

namespace Dematt.Airy.Tests.NodaTime.Entities
{
    public class LocalTimeTestEntity : EntityWithIntId
    {
        public virtual string Description { get; set; }

        public virtual LocalTime StartLocalTime { get; set; }

        public virtual LocalTime? FinishLocalTime { get; set; }
    }
}
