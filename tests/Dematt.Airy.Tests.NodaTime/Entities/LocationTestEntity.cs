using Dematt.Airy.Core;
using NodaTime;

namespace Dematt.Airy.Tests.NodaTime.Entities
{
    public class LocationTestEntity : EntityWithIntId
    {
        public virtual string Description { get; set; }

        public virtual DateTimeZone LocationDateTimeZone { get; set; }
    }
}
