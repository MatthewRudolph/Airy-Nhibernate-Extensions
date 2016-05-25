using System;
using Dematt.Airy.Core;

namespace Dematt.Airy.Tests.NodaTime.Entities
{
    public class DateTimeOffsetTestEntity : EntityWithIntId
    {
        public virtual string Description { get; set; }

        public virtual DateTimeOffset StartDateTimeOffset { get; set; }

        public virtual DateTimeOffset FinishDateTimeOffset { get; set; }

        public virtual LocationTestEntity EventLocation { get; set; }
    }
}
