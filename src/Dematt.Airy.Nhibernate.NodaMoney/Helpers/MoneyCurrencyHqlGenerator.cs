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
    public class MoneyCurrencyHqlGenerator : BaseHqlGeneratorForMethod
    {
        private static readonly HashSet<MethodInfo> ActingMethods = new HashSet<MethodInfo>
        {
            // The method calls are used only to get info about the signature, any parameters are just ignored.
            ReflectionHelper.GetMethodDefinition<Money>(x => x.GetCurrencyCode())            
        };

        public MoneyCurrencyHqlGenerator()
        {
            SupportedMethods = ActingMethods;
        }


        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments,
            HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var source = visitor.Visit(arguments[0]).AsExpression();
            var property = treeBuilder.Ident("Currency");
            return treeBuilder.Dot(source, property);
        }
    }
}
