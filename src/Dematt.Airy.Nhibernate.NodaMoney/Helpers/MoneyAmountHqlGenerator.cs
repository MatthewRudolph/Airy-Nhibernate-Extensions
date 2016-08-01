using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NodaMoney;

namespace Dematt.Airy.Nhibernate.NodaMoney.Helpers
{
    /// <summary>
    /// Class that extends the NHiberate Linq Provider to allow the Money.Amount property to be used in Linq queries.
    /// </summary>
    /// <remarks>Use the <see cref="LinqToHqlRegisterMoney"/> class to register this generator with Nhibernate.</remarks>
    public class MoneyAmountHqlGenerator : BaseHqlGeneratorForProperty
    {
        /// <summary>
        /// Gets the properties that are supported by this Linq extension.
        /// </summary>
        private static readonly HashSet<MemberInfo> SelectedProperties = new HashSet<MemberInfo>
        {
            typeof(Money).GetProperty("Amount")            
        };

        /// <summary>
        /// Constructor sets the supported properties.
        /// </summary>
        public MoneyAmountHqlGenerator()
        {
            SupportedProperties = SelectedProperties;
        }

        /// <summary>
        /// Overrides the BuildHql method to add an expression that supports Linq querying of the supported properties. 
        /// </summary>
        public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var source = visitor.Visit(expression).AsExpression();
            var property = treeBuilder.Ident("Amount");
            return treeBuilder.Dot(source, property);
        }
    }
}
