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
    /// <summary>
    /// Class that extends the NHiberate Linq Provider to allow the Money.ToAmount Extension Method to be used in Linq queries.
    /// </summary>
    /// <remarks>Use the <see cref="LinqToHqlRegisterMoney"/> class to register this generator with Nhibernate.</remarks>
    public class MoneyToAmountHqlGenerator : BaseHqlGeneratorForMethod
    {
        /// <summary>
        /// Gets the methods that are supported by this Linq extension.
        /// </summary>
        private static readonly HashSet<MethodInfo> ActingMethods = new HashSet<MethodInfo>
        {
            // The method calls are used only to get info about the signature, any parameters are just ignored.
            ReflectionHelper.GetMethodDefinition<Money>(x => x.ToAmount())            
        };

        /// <summary>
        /// Constructor sets the supported methods.
        /// </summary>
        public MoneyToAmountHqlGenerator()
        {
            SupportedMethods = ActingMethods;
        }

        /// <summary>
        /// Overrides the BuildHql method to add an expression that supports Linq querying of the supported methods.
        /// </summary>
        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments,
            HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var source = visitor.Visit(arguments[0]).AsExpression();
            var property = treeBuilder.Ident("Amount");
            return treeBuilder.Dot(source, property);
        }
    }
}
