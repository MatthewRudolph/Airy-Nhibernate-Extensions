using NHibernate.Engine;
using NHibernate.UserTypes;

namespace Dematt.Airy.Nhibernate.NodaMoney
{
    /// <summary>
    /// Abstract base class for classes implementing the <see cref="ICompositeUserType"/>
    /// </summary>
    /// <remarks>
    /// Implements the following methods:
    ///     object DeepCopy(object value)
    ///     object Replace(object original, ISessionImplementor session, object target, object owner)
    ///     object Assemble(object cached, ISessionImplementor session, object owner)
    ///     object Disassemble(object value, ISessionImplementor session)
    /// </remarks>
    public abstract class ImmutableCompositeUserType
    {
        /// <summary>
        /// Are objects of this type mutable? (No in this case)
        /// </summary>
        /// <remarks>
        /// This class is for immutable types only so always return false.
        /// </remarks>
        public bool IsMutable
        {
            // As our object is immutable just return false.
            get { return false; }
        }

        /// <summary>
        /// Return a deep copy of the persistent state, stopping at entities and at collections.
        /// </summary>        
        /// <param name="value">The object to create a deep copy of.</param>
        /// <returns>A new object with the same values as the original object.</returns>     
        /// <remarks>
        /// For immutable objects we can just return the original value.
        /// </remarks>
        public object DeepCopy(object value)
        {
            // As our object is immutable just return the value.
            return value;
        }

        /// <summary>
        /// During merge, replace the existing (<paramref name="target"/>) value in the entity
        /// we are merging to with a new (<paramref name="original"/>) value from the detached
        /// entity we are merging. 
        /// </summary>
        /// <param name="original">The value from the detached entity being merged.</param>
        /// <param name="target">The value in the managed entity.</param>
        /// <param name="session">The session.</param>
        /// <param name="owner">The managed entity.</param>
        /// <returns>The value to be merged.</returns>
        /// <remarks>
        /// For immutable objects, or null values, it is safe to simply return the first parameter.
        /// For mutable objects, it is safe to return a copy of the first parameter.
        /// For objects with component values, it might make sense to recursively replace component values.        
        /// </remarks>
        public object Replace(object original, object target, ISessionImplementor session, object owner)
        {
            // As our object is immutable we can just return the original.
            return original;
        }

        /// <summary>
        /// Reconstruct an object from the cacheable representation.
        /// </summary>        
        /// <param name="cached">The object to be cached.</param>
        /// <param name="session">The session.</param>
        /// <param name="owner">The owner of the cached object.</param>
        /// <returns>A reconstructed object from the cacheable representation.</returns>
        /// <remarks>
        /// At the very least this method should perform a deep copy if the type is mutable.  (Optional operation)
        /// </remarks>
        public object Assemble(object cached, ISessionImplementor session, object owner)
        {
            return DeepCopy(cached);
        }

        /// <summary>
        /// Transform the object into its cacheable representation.         
        /// </summary>
        /// <param name="value">The object to be cached.</param>
        /// <param name="session">The session.</param>
        /// <returns>A cacheable representation of the object.</returns>
        /// <remarks>
        /// At the very least this method should perform a deep copy if the type is mutable.
        /// That may not be enough for some implementations, however; 
        /// for example, associations must be cached as identifier values. (Optional operation)
        /// </remarks>
        public object Disassemble(object value, ISessionImplementor session)
        {
            return DeepCopy(value);
        }
    }
}
