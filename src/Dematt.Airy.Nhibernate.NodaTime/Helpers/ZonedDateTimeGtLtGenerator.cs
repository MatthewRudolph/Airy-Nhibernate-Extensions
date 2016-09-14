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
    /// <summary>
    /// Class that extends the NHiberate Linq Provider to allow the ZonedDateTime.ToDateTimeOffset() method to be used in Linq queries.
    /// </summary>
    /// <remarks>Use the <see cref="LinqToHqlRegisterNodaTime"/> class to register this generator with Nhibernate.</remarks>
    [SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public class ZonedDateTimeGtLtGenerator : BaseHqlGeneratorForMethod
    {
        /// <summary>
        /// Gets the methods that are supported by this Linq extension.
        /// </summary>
        private static readonly HashSet<MethodInfo> ActingMethods = new HashSet<MethodInfo>
        {
            // The method calls are used only to get info about the signature, any parameters are just ignored.
            ReflectionHelper.GetMethodDefinition<ZonedDateTime>(x => x.ToDateTimeOffset()),
            
            // Not sure how to add CompareTo (> , =>, < and <=) to be supported methods.
            // From the NHibernate source of the CompareGenerator class the below should work but it does not,
            // possible something to do with the way ZonedDateTime implements CompareTo?
            //ReflectionHelper.GetMethodDefinition<ZonedDateTime>(x => x.CompareTo(x))
        };

        /// <summary>
        /// Constructor sets the supported properties.
        /// </summary>
        public ZonedDateTimeGtLtGenerator()
        {
            SupportedMethods = ActingMethods.ToArray();

            //SupportedMethods = typeof(ZonedDateTime).GetMethods().Where(m => m.Name == "CompareTo").ToArray();
            //SupportedMethods = typeof(ZonedDateTime).GetMethods().Where(m => m.Name.Contains("Than")).ToArray();
        }

        /// <summary>
        /// Overrides the BuildHql method to add an expression that supports Linq querying using the supported methods.
        /// </summary>
        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments,
            HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            var source = visitor.Visit(targetObject).AsExpression();
            var property = treeBuilder.Ident("DateTimeOffset");
            return treeBuilder.Dot(source, property);
        }
    }
}
