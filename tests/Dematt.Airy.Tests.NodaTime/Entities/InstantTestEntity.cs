using Dematt.Airy.Core;
using NodaTime;

namespace Dematt.Airy.Tests.NodaTime.Entities
{
    public class InstantTestEntity : EntityWithIntId
    {
        public virtual string Description { get; set; }

        public virtual Instant StartInstant { get; set; }

        public virtual Instant FinishInstant { get; set; }
    }
}
