using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Hql.Ast;
using NHibernate.Linq;
using NHibernate.Linq.Functions;
using NHibernate.Linq.Visitors;

namespace Dematt.Airy.Nhibernate.Extensions.Linq
{

    /// <summary>
    /// Class that extends the NHiberate Linq Provider to allow the object.IsNotNull() extension method to be used in Linq queries
    /// and recognize the method as something that can be performed in the database, 
    /// replacing the load of a whole entity to check for null with an single extra column in query
    /// </summary>
    /// <remarks>Use the <see cref="LinqToHqlRegisterIsNotNull"/> class to register this generator with Nhibernate.</remarks>
    public class IsNotNullGenerator : BaseHqlGeneratorForMethod
    {
        /// <summary>
        /// Constructor sets the supported properties.
        /// </summary>
        public IsNotNullGenerator()
        {
            SupportedMethods = new[]
            {
                // The method calls are used only to get info about the signature, any parameters are just ignored.                
                ReflectionHelper.GetMethodDefinition<object>(x => x.IsNotNull())
            };
        }

        /// <summary>
        /// Overrides the BuildHql method to add an expression that supports Linq querying using the supported methods.
        /// </summary>
        public override HqlTreeNode BuildHql(MethodInfo method, Expression targetObject, ReadOnlyCollection<Expression> arguments,
            HqlTreeBuilder treeBuilder, IHqlExpressionVisitor visitor)
        {
            return treeBuilder.IsNotNull(visitor.Visit(arguments[0]).AsExpression());
        }
    }
}
