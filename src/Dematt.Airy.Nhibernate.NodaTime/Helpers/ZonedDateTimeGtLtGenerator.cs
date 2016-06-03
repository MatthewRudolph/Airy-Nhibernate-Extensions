using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NodaTime;

namespace Dematt.Airy.Nhibernate.NodaTime.Helpers
{
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public class ZonedDateTimeGtLtGenerator : BaseHqlGeneratorForMethod
    {
        private static readonly HashSet<MethodInfo> ActingMethods = new HashSet<MethodInfo>
        {
            // The method calls are used only to get info about the signature, any parameters are just ignored.
            ReflectionHelper.GetMethodDefinition<ZonedDateTime>(x => x.ToDateTimeOffset()),
            
            // Not sure how to add CompareTo (> , =>, < and <=) to be supported methods.
            // From the NHibernate source of the CompareGenerator class the below should work but it does not,
            // possible something to do with the way ZonedDateTime implements CompareTo?
            //ReflectionHelper.GetMethodDefinition<ZonedDateTime>(x => x.CompareTo(x))
        };

        public ZonedDateTimeGtLtGenerator()
        {
            SupportedMethods = ActingMethods.ToArray();
        }

        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments,
            HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var source = visitor.Visit(targetObject).AsExpression();
            var property = treeBuilder.Ident("DateTimeOffset");
            return treeBuilder.Dot(source, property);
        }
    }
}
