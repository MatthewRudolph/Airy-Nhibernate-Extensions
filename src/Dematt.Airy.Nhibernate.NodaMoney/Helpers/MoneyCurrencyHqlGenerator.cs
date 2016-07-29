using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NodaMoney;

namespace Dematt.Airy.Nhibernate.NodaMoney.Helpers
{
    public class MoneyCurrencyHqlGenerator : BaseHqlGeneratorForProperty
    {
        private static readonly HashSet<MemberInfo> SelectedProperties = new HashSet<MemberInfo>
        {
            typeof(Money).GetProperty("Currency")
            //typeof(Money).GetProperty("Currency").GetType().GetProperty("Code")
        };

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

        public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var source = visitor.Visit(expression).AsExpression();
            var property = treeBuilder.Ident("Currency");
            return treeBuilder.Dot(source, property);
        }
    }
}
