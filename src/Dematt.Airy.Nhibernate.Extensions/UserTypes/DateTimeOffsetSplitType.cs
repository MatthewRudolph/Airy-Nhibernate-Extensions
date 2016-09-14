using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;

namespace Dematt.Airy.Nhibernate.Extensions.UserTypes
{
    /// <summary>
    /// An NHibernate Composite User Type for the <see cref="DateTimeOffset"/> class.
    /// </summary>
    /// <remarks>
    /// This allows database such as SQLite that do not natively support the DateTimeOffset data type
    /// to store it in two separate columns one for the date and time and another for the offset.
    /// </remarks>
    public class DateTimeOffsetSplitType : ICompositeUserType
    {
        /// <summary>
        /// Gets a value indicating whether objects of this type are mutable.
        /// </summary>
        public bool IsMutable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the "property names" that may be used in a query.
        /// </summary>
        public string[] PropertyNames
        {
            get
            {
                return new[] { "DateTime", "Offset" };
            }
        }

        /// <summary>
        /// Gets the corresponding "property types".
        /// </summary>
        public IType[] PropertyTypes
        {
            get
            {
                return new IType[] { NHibernateUtil.DateTime, NHibernateUtil.Int32 };
            }
        }

        /// <summary>
        /// Gets the type of the class returned by NullSafeGet().
        /// </summary>
        public Type ReturnedClass
        {
            get
            {
                return typeof(DateTimeOffset?);
            }
        }

        /// <summary>
        /// Gets the value of a property.
        /// </summary>
        /// <param name="component">An instance of the class mapped by this "type".</param>
        /// <param name="property">The index of the property to get.</param>
        /// <returns>The property value.</returns>
        public object GetPropertyValue(object component, int property)
        {
            var dateTimeOffset = AsDateTimeOffset(component);
            if (dateTimeOffset == null)
            {
                return null;
            }

            switch (property)
            {
                case 0:
                    return dateTimeOffset.Value.DateTime;
                case 1:
                    return dateTimeOffset.Value.Offset.TotalMinutes;
                default:
                    throw new Exception(string.Format("No implementation for property index of '{0}'.", property));
            }
        }

        /// <summary>
        /// Set the value of a property.
        /// </summary>
        /// <param name="component">An instance of class mapped by this "type".</param>
        /// <param name="property">The index of the property to set.</param>
        /// <param name="value">The value to set.</param>
        public void SetPropertyValue(object component, int property, object value)
        {
            throw new InvalidOperationException("DateTimeOffset is an immutable object. SetPropertyValue isn't supported.");
        }

        /// <summary>
        /// Compare two instances of the class mapped by this type for persistence
        /// "equality", i.e. equality of persistent state.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>true if the objects are considered equal; otherwise, false. If both x and y are null, the method returns true.</returns>
        [SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1503:CurlyBracketsMustNotBeOmitted", Justification = "Reviewed. Suppression is OK here.")]
        public new bool Equals(object x, object y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        /// <summary>
        /// Gets a hash code for the instance, consistent with persistence "equality"
        /// </summary>
        /// <param name="x">The object to get the hash code for.</param>
        /// <returns>A hash code for the object x.</returns>
        public int GetHashCode(object x)
        {
            return x == null ? 0 : x.GetHashCode();
        }

        /// <summary>
        /// Retrieve an instance of the mapped class from a IDataReader. We need to handle possibility of null values.
        /// </summary>
        /// <param name="dr">An object that implements IDataReader</param>
        /// <param name="names">The column names</param>
        /// <param name="session">The session</param>
        /// <param name="owner">The containing entity</param>
        /// <returns>An instance of the <see cref="DateTimeOffset"/> class or null.</returns>
        public object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner)
        {
            var dateTime = NHibernateUtil.DateTime.NullSafeGet(dr, names[0]);
            var offset = NHibernateUtil.Int32.NullSafeGet(dr, names[1]);
            if (dateTime == null)
            {
                return null;
            }

            var timeSpan = offset == null ? new TimeSpan(0, 0, 0) : new TimeSpan(0, (int)offset, 0);
            return new DateTimeOffset((DateTime)dateTime, timeSpan);
        }

        /// <summary>
        /// Write an instance of the mapped class to a prepared statement. We should handle possibility of null values.
        /// A multi-column type should be written to parameters starting from index.
        /// If a property is not settable, skip it and don't increment the index.
        /// </summary>
        /// <param name="cmd">The command used for writing the value.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="index">The parameters index to start at.</param>
        /// <param name="settable">Array indicating which properties are settable</param>
        /// <param name="session">The session.</param>
        public void NullSafeSet(IDbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
        {
            if (value == null)
            {
                ((IDataParameter)cmd.Parameters[index]).Value = DBNull.Value;
                ((IDataParameter)cmd.Parameters[index + 1]).Value = DBNull.Value;
            }
            else
            {
                var dateTimeOffset = AsDateTimeOffset(value);
                ((IDataParameter)cmd.Parameters[index]).Value =
                    dateTimeOffset == null ? DBNull.Value : (object)dateTimeOffset.Value.DateTime;
                ((IDataParameter)cmd.Parameters[index + 1]).Value =
                    dateTimeOffset == null ? DBNull.Value : (object)dateTimeOffset.Value.Offset.TotalMinutes;
            }
        }

        /// <summary>
        /// Return a deep copy of the persistent state, stopping at entities and at collections.
        /// </summary>
        /// <param name="value">The object to create a deep copy of.</param>
        /// <returns>A new object with the same values as the original object.</returns>
        public object DeepCopy(object value)
        {
            if (value == null)
            {
                return null;
            }

            return new DateTimeOffset(((DateTimeOffset)value).DateTime, ((DateTimeOffset)value).Offset);
        }

        /// <summary>
        /// Transform the object into its cacheable representation.
        /// </summary>
        /// <remarks>
        /// At the very least this method should perform a deep copy, which is enough in the case of this DateTimeOffset struct.
        /// That may not be enough for some implementations, however; for example, associations must be cached as identifier values. (optional operation)
        /// </remarks>
        /// <param name="value">The object to be cached</param>
        /// <param name="session">The session.</param>
        /// <returns>A cacheable representation of the object value.</returns>
        public object Disassemble(object value, ISessionImplementor session)
        {
            return DeepCopy(value);
        }

        /// <summary>
        /// Reconstruct an object from the cacheable representation.
        /// </summary>
        /// <remarks>
        /// At the very least this method should perform a deep copy. (optional operation)
        /// Which is enough in the case of this DateTimeOffset struct.
        /// </remarks>        
        /// <param name="cached">A cached version of the object.</param>
        /// <param name="session">The session.</param>
        /// <param name="owner">The owner.</param>
        /// <returns>A new version of the object taken from the cache.</returns>
        public object Assemble(object cached, ISessionImplementor session, object owner)
        {
            return DeepCopy(cached);
        }

        /// <summary>
        /// During merge, replace the existing (target) value in the entity we are merging to
        /// with a new (original) value from the detached entity we are merging.
        /// </summary>
        /// <remarks>
        /// For immutable objects, or null values, it is safe to simply return the first parameter.
        /// For mutable objects, it is safe to return a copy of the first parameter. 
        /// However, since composite user types often define component values,
        /// it might make sense to recursively replace component values in the target object.
        /// </remarks>
        /// <param name="original">The original object.</param>
        /// <param name="target">The target object.</param>
        /// <param name="session">The session.</param>
        /// <param name="owner">The owner.</param>
        /// <returns>The object original.</returns>
        public object Replace(object original, object target, ISessionImplementor session, object owner)
        {
            return original;
        }

        /// <summary>
        /// Tries to cast an unknown object to an <see cref="DateTimeOffset"/> object.
        /// </summary>
        /// <param name="value">The object to try to cast to a <see cref="DateTimeOffset"/> object.</param>
        /// <returns>A <see cref="DateTimeOffset"/> object if the value can be cast.</returns>
        private static DateTimeOffset? AsDateTimeOffset(object value)
        {
            if (value == null)
            {
                return null;
            }

            var ts = value as DateTimeOffset?;
            if (ts == null)
            {
                throw new InvalidCastException(
                    string.Format("Expected '{0}' but received '{1}'.", typeof(DateTimeOffset?), value.GetType()));
            }

            return ts;
        }
    }
}
