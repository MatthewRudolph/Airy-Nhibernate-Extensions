using System;
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
    /// This class has never worked as I can't get the expression to refer to the Code property of the Currency of the Money instance in the correct way.
    /// And/Or build the correct HqlTreeNode.
    /// It has been left here, just in case someone else in the future wants to implement it correctly :-)
    /// </summary>
    /// <remarks>
    /// This is not currently register by the <see cref="LinqToHqlRegisterMoney"/> class.
    /// If someone does fix it then remember to add a line to register it in that class.
    /// </remarks>
    [Obsolete("This has never worked and is not supported.  Please do not use.")]
    public class MoneyCurrencyHqlGenerator : BaseHqlGeneratorForProperty
    {
        /// <summary>
        /// Gets the properties that are supported by this Linq extension.
        /// </summary>
        private static readonly HashSet<MemberInfo> SelectedProperties = new HashSet<MemberInfo>
        {
            typeof(Money).GetProperty("Currency")
            //typeof(Money).GetProperty("Currency").GetType().GetProperty("Code")
        };

        /// <summary>
        /// Constructor sets the supported properties.
        /// </summary>
        public MoneyCurrencyHqlGenerator()
        {
            // **Use a Func to get the property... (Nope :-()**
            //Expression<Func<Money, string>> func1 = p => p.Currency.Code;
            //SupportedProperties = new List<MemberInfo> { GetMemberInfo(func1) };

            // **Use GetProperty to get the property... (Nope :-()**
            //var currency = typeof(Money).GetProperty("Currency");
            //var code = currency.PropertyType.GetProperty("Code");
            //SupportedProperties = new List<MemberInfo>() { code };

            // **Get the supported property from the static SelectedProperties HashSet declared above.**
            SupportedProperties = SelectedProperties;
        }

        /// <summary>
        /// Overrides the BuildHql method to add an expression that supports Linq querying of the supported properties. 
        /// </summary>
        public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var source = visitor.Visit(expression).AsExpression();
            var property = treeBuilder.Ident("Currency");
            return treeBuilder.Dot(source, property);
        }
    }
}
