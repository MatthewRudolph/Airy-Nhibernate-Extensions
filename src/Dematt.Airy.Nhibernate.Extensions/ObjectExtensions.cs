using System;

namespace Dematt.Airy.Nhibernate.Extensions
{
    public static class ObjectExtensions
    {


        /// <summary>
        /// Returns true if the type of this objects is a subclass of the unbound generic passed, otherwise returns false.
        /// </summary>
        /// <remarks>
        /// Ref: http://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class
        /// </remarks>
        public static bool IsSubclassOfUnboundGeneric(this object obj, Type unboundGeneric)
        {
            Type typeToCheck = obj.GetType();

            if (typeToCheck == unboundGeneric)
            {
                return false;
            }

            while (typeToCheck != null && typeToCheck != typeof(object))
            {
                var cur = typeToCheck.IsGenericType ? typeToCheck.GetGenericTypeDefinition() : typeToCheck;
                if (unboundGeneric == cur)
                {
                    return true;
                }
                typeToCheck = typeToCheck.BaseType;
            }
            return false;
        }

        /// <summary>
        /// Returns true if the object is not null, otherwise false.
        /// </summary>
        /// <remarks>
        /// This is useful as it allows us to extend the NHibernate Linq provider to recognize our method as a something 
        /// that can be performed in the database and replace load of a whole entity to check for null with an single extra column in query.
        /// Ref: http://jahav.com/blog/nhibernate-linq-query-evaluation-process/
        /// Ref: http://stackoverflow.com/questions/32215679/checking-for-reference-entity-being-null-in-select-projects-all-columns-in
        /// Ref: http://stackoverflow.com/questions/21126767/extension-method-to-check-if-an-instance-is-null
        /// </remarks>
        public static bool IsNotNull<T>(this T obj) where T : class
        {
            return !ReferenceEquals(obj, null);
        }
    }
}
