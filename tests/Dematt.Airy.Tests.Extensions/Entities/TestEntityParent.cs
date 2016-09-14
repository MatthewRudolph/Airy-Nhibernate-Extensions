using System;
using Dematt.Airy.Core;

namespace Dematt.Airy.Tests.Extensions.Entities
{
    public class TestEntityParent : EntityWithId<Guid>
    {
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual TestEntityChild Child { get; set; }
    }
}
