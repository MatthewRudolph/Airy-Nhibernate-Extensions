using System;
using Dematt.Airy.Core;

namespace Dematt.Airy.Tests.NodaTime.Entities
{
    public class TestEvent : EntityWithIntId
    {
        public virtual string Description { get; set; }

        public virtual DateTimeOffset SystemDateTimeOffset { get; set; }
    }
}
