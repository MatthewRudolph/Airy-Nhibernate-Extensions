using System;
using Dematt.Airy.Core;
using NodaMoney;

namespace Dematt.Airy.Tests.NodaMoney.Entities
{
    public class MoneyTestEntity : EntityWithId<Guid>
    {
        public virtual string Description { get; set; }

        public virtual Money Cost { get; set; }

        public virtual Money? Retail { get; set; }
    }
}
