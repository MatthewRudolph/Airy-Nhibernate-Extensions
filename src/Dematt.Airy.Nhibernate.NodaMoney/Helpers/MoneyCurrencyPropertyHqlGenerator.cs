using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using Dematt.Airy.Nhibernate.NodaMoney.Extensions;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;
using NodaMoney;

namespace Dematt.Airy.Nhibernate.NodaMoney.Helpers
{
    public class MoneyCurrencyPropertyHqlGenerator : BaseHqlGeneratorForProperty
    {
        private static readonly HashSet<MemberInfo> SelectedProperties = new HashSet<MemberInfo>
        {
            //typeof(Money).GetProperty("Currency").GetType().GetProperty("Code")
        };

        public MoneyCurrencyPropertyHqlGenerator()
        {
            Expression<Func<Money, string>> func1 = p => p.Currency.Code;
            SupportedProperties = new List<MemberInfo> { GetMemberInfo(func1) };

            //var currency = typeof(Money).GetProperty("Currency");
            //var code = currency.PropertyType.GetProperty("Code");
            //SupportedProperties = new List<MemberInfo>() { code };
        }

        public override HqlTreeNode BuildHql(MemberInfo member, Expression expression, HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var source = visitor.Visit(expression).AsExpression();
            var property = treeBuilder.Ident("Amount");
            return treeBuilder.Dot(source, property);
        }

        public static MemberInfo GetMemberInfo(LambdaExpression exp)
        {
            var body = exp.Body as MemberExpression;
            return body.Member;
        }
    }
}
