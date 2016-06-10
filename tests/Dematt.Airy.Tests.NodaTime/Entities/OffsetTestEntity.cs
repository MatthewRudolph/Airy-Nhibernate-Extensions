using Dematt.Airy.Core;
using NodaTime;

namespace Dematt.Airy.Tests.NodaTime.Entities
{
    public class OffsetTestEntity : EntityWithIntId
    {
        public virtual string Description { get; set; }

        public virtual Offset StartOffset { get; set; }

        public virtual Offset? FinishOffset { get; set; }
    }
}
