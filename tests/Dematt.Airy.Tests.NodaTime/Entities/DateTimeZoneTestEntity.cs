using Dematt.Airy.Core;
using NodaTime;

namespace Dematt.Airy.Tests.NodaTime.Entities
{
    public class DateTimeZoneTestEntity : EntityWithIntId
    {
        public virtual string Description { get; set; }

        public virtual DateTimeZone StartDateTimeZone { get; set; }
    }
}
