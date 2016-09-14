using System;
using Dematt.Airy.Core;

namespace Dematt.Airy.Tests.Extensions.Entities
{
    public class TestEntityChild : EntityWithId<Guid>
    {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual int NumberOfApples { get; set; }
    }
}
