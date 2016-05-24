using System;
using System.Collections.ObjectModel;
using System.Reflection;
using NHibernate.Criterion;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NodaTime;

namespace Dematt.Airy.Nhibernate.NodaTime
{
    // Converts OffsetDateTime.ToInstant() to use the OffsetDateTimeType's UtcDateTime property
    // e.g. items.Where(x => x.Date.ToInstant() > minDate.ToInstant())
    // is converted to the HQL; WHERE x.UtcDateTime > @minDate
    public class OffsetDateTimeToInstantGenerator : BaseHqlGeneratorForMethod
    {
        public OffsetDateTimeToInstantGenerator()
        {
            SupportedMethods = new[] {
            ReflectionHelper.GetMethodDefinition<OffsetDateTime> (x => x.ToDateTimeOffset())
        };
        }

        public override HqlTreeNode BuildHql(MethodInfo method, System.Linq.Expressions.Expression targetObject,
          ReadOnlyCollection<System.Linq.Expressions.Expression> arguments, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var source = visitor.Visit(targetObject).AsExpression();
            var property = treeBuilder.Ident("FinishOffsetDateTime");
            return treeBuilder.Dot(source, property);
        }
    }
}
