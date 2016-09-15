using System;
using System.Linq;
using Dematt.Airy.Nhibernate.Extensions;
using Dematt.Airy.Tests.Extensions.Entities;
using Dematt.Airy.Tests.Extensions.Models;
using NHibernate;
using NHibernate.Linq;
using NUnit.Framework;

namespace Dematt.Airy.Tests.Extensions
{
    public class IsNullExtensionTests : PersistenceTest
    {
        /// <summary>
        /// Can we save a simple parent entity.
        /// </summary>
        [Test]
        public void Can_Write_Simple_Parent_Entity()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var parent = new TestEntityParent
                {
                    Name = "Can_Write_Simple_Parent_Entity",
                    Description = "Desc: Can_Write_Simple_Parent_Entity"
                };

                session.Save(parent);
                transaction.Commit();
                Assert.That(parent.Id, Is.Not.Null);
            }
        }

        /// <summary>
        /// Can we save a parent entity with a child.
        /// </summary>
        [Test]
        public void Can_Write_Parent_Entity_With_Child()
        {
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var parent = new TestEntityParent
                {
                    Name = "Can_Write_Parent_Entity_With_Child",
                    Description = "Desc: Can_Write_Parent_Entity_With_Child",
                    Child = new TestEntityChild
                    {
                        Name = "Can_Write_Parent_Entity_With_Child - Child",
                        Description = "Desc: Can_Write_Parent_Entity_With_Child - Child",
                        NumberOfApples = 58256897
                    }
                };

                session.Save(parent);
                transaction.Commit();
                Assert.That(parent.Id, Is.Not.Null);
            }
        }

        /// <summary>
        /// What sql do we get when we read a parent with a child into models without checking the child for null.
        /// </summary>
        [Test]
        public void Can_Read_Parent_Entity_With_Child_No_Null_Check()
        {
            Guid parentId;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var parent = new TestEntityParent
                {
                    Name = "Can_Read_Parent_Entity_With_Child_No_Null_Check",
                    Description = "Desc: Can_Read_Parent_Entity_With_Child_No_Null_Check",
                    Child = new TestEntityChild
                    {
                        Name = "Can_Read_Parent_Entity_With_Child_No_Null_Check - Child",
                        Description = "Desc: Can_Read_Parent_Entity_With_Child_No_Null_Check - Child",
                        NumberOfApples = 62256897
                    }
                };

                session.Save(parent);
                transaction.Commit();
                parentId = parent.Id;
                Assert.That(parent.Id, Is.Not.Null);
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<TestEntityParent>()
                    .Where(x => x.Id == parentId)
                    .Select(m => new ParentModel
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Child = new ChildModel { Id = m.Child.Id, NumberOfApples = m.Child.NumberOfApples }
                    });

                ParentModel parentModel = query.Single();
                Assert.That(parentModel.Id, Is.Not.Null);
                Assert.That(parentModel.Child.NumberOfApples, Is.GreaterThan(0));

                transaction.Commit();
            }
        }

        /// <summary>
        /// What sql to we get when we read a parent with a child into models checking the child for null using != null       
        /// </summary>
        [Test]
        public void Can_Read_Parent_Entity_With_Child_Null_Check()
        {
            Guid parentId;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var parent = new TestEntityParent
                {
                    Name = "Can_Read_Parent_Entity_With_Child_Null_Check",
                    Description = "Desc: Can_Read_Parent_Entity_With_Child_Null_Check",
                    Child = new TestEntityChild
                    {
                        Name = "Can_Read_Parent_Entity_With_Child_Null_Check - Child",
                        Description = "Desc: Can_Read_Parent_Entity_With_Child_Null_Check - Child",
                        NumberOfApples = 68256897
                    }
                };

                session.Save(parent);
                transaction.Commit();
                parentId = parent.Id;
                Assert.That(parent.Id, Is.Not.Null);
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<TestEntityParent>()
                    .Where(x => x.Id == parentId)
                    .Select(m => new ParentModel
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Child = m.Child != null ? new ChildModel { Id = m.Child.Id, NumberOfApples = m.Child.NumberOfApples } : null
                    });

                ParentModel parentModel = query.Single();
                Assert.That(parentModel.Id, Is.Not.Null);
                Assert.That(parentModel.Child.NumberOfApples, Is.GreaterThan(0));

                transaction.Commit();
            }
        }

        /// <summary>
        /// What sql to we get when we read a parent with a child into models checking the child for null using the IsNotNull extension method.
        /// </summary>
        [Test]
        public void Can_Read_Parent_Entity_With_Child_IsNotNull_Extension()
        {
            Guid parentId;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var parent = new TestEntityParent
                {
                    Name = "Can_Read_Parent_Entity_With_Child_IsNotNull_Extension",
                    Description = "Desc: Can_Read_Parent_Entity_With_Child_IsNotNull_Extension",
                    Child = new TestEntityChild
                    {
                        Name = "Can_Read_Parent_Entity_With_Child_IsNotNull_Extension - Child",
                        Description = "Desc: Can_Read_Parent_Entity_With_Child_IsNotNull_Extension - Child",
                        NumberOfApples = 87256897
                    }
                };

                session.Save(parent);
                transaction.Commit();
                parentId = parent.Id;
                Assert.That(parent.Id, Is.Not.Null);
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<TestEntityParent>()
                    .Where(x => x.Id == parentId)
                    .Select(m => new ParentModel
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Child = m.Child.IsNotNull() ? new ChildModel { Id = m.Child.Id, NumberOfApples = m.Child.NumberOfApples } : null
                    });

                ParentModel parentModel = query.Single();
                Assert.That(parentModel.Id, Is.Not.Null);
                Assert.That(parentModel.Child.NumberOfApples, Is.GreaterThan(0));

                transaction.Commit();
            }
        }

        /// <summary>
        /// What sql to we get when we read a parent without a child into models checking the child for null using != null       
        /// </summary>
        [Test]
        public void Can_Read_Parent_Entity_Without_Child_Null_Check()
        {
            Guid parentId;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var parent = new TestEntityParent
                {
                    Name = "Can_Read_Parent_Entity_Without_Child_Null_Check",
                    Description = "Desc: Can_Read_Parent_Entity_Without_Child_Null_Check"
                };

                session.Save(parent);
                transaction.Commit();
                parentId = parent.Id;
                Assert.That(parent.Id, Is.Not.Null);
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<TestEntityParent>()
                    .Where(x => x.Id == parentId)
                    .Select(m => new ParentModel
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Child = m.Child != null ? new ChildModel { Id = m.Child.Id, NumberOfApples = m.Child.NumberOfApples } : null
                    });

                ParentModel parentModel = query.Single();
                Assert.That(parentModel.Id, Is.Not.Null);
                Assert.That(parentModel.Child, Is.Null);

                transaction.Commit();
            }
        }

        /// <summary>
        /// What sql to we get when we read a parent without a child into models checking the child for null using the IsNotNull extension method.
        /// </summary>
        [Test]
        public void Can_Read_Parent_Entity_Without_Child_IsNotNull_Extension()
        {
            Guid parentId;
            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var parent = new TestEntityParent
                {
                    Name = "Can_Read_Parent_Entity_Without_Child_IsNotNull_Extension",
                    Description = "Desc: Can_Read_Parent_Entity_Without_Child_IsNotNull_Extension"
                };

                session.Save(parent);
                transaction.Commit();
                parentId = parent.Id;
                Assert.That(parent.Id, Is.Not.Null);
            }

            using (ISession session = SessionFactory.OpenSession())
            using (ITransaction transaction = session.BeginTransaction())
            {
                var query = session.Query<TestEntityParent>()
                    .Where(x => x.Id == parentId)
                    .Select(m => new ParentModel
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Child = m.Child.IsNotNull() ? new ChildModel { Id = m.Child.Id, NumberOfApples = m.Child.NumberOfApples } : null
                    });

                ParentModel parentModel = query.Single();
                Assert.That(parentModel.Id, Is.Not.Null);
                Assert.That(parentModel.Child, Is.Null);

                transaction.Commit();
            }
        }
    }
}
