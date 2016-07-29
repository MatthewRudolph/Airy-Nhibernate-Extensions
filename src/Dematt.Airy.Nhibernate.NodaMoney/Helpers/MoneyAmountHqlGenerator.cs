using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NodaMoney;

namespace Dematt.Airy.Nhibernate.NodaMoney.Helpers
{
    public class MoneyAmountHqlGenerator : BaseHqlGeneratorForProperty
    {
        private static readonly HashSet<MemberInfo> SelectedProperties = new HashSet<MemberInfo>
        {
            typeof(Money).GetProperty("Amount")            
        };

        public MoneyAmountHqlGenerator()
        {
            SupportedProperties = SelectedProperties;
        }

        public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var source = visitor.Visit(expression).AsExpression();
            var property = treeBuilder.Ident("Amount");
            return treeBuilder.Dot(source, property);
        }
    }
}
