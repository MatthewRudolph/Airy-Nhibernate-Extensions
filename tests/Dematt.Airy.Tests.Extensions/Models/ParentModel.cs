using System;

namespace Dematt.Airy.Tests.Extensions.Models
{
    public class ParentModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public ChildModel Child { get; set; }
    }
}
