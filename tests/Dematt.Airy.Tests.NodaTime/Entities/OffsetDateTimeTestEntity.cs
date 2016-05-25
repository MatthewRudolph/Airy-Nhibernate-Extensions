using System;
using Dematt.Airy.Core;
using NodaTime;

namespace Dematt.Airy.Tests.NodaTime.Entities
{
    public class OffsetDateTimeTestEntity : EntityWithIntId
    {
        public virtual string Description { get; set; }

        public virtual DateTimeOffset SystemDateTimeOffset { get; set; }

        public virtual OffsetDateTime? StartOffsetDateTime { get; set; }

        public virtual OffsetDateTime FinishOffsetDateTime { get; set; }
    }
}
