using System;
using System.Data;
using Dematt.Airy.Nhibernate.NodaTime.Extensions;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaTime;

namespace Dematt.Airy.Nhibernate.NodaTime
{
    public class ZonedDateTimeType : ICompositeUserType
    {
        /// <summary>
        /// Gets the "property names" that may be used in a query.
        /// </summary>
        public string[] PropertyNames
        {
            get
            {
                return new[] { "DateTimeOffset", "DateTimeZoneId" };
            }
        }

        /// <summary>
        /// Gets the corresponding "property types".
        /// </summary>
        public IType[] PropertyTypes
        {
            get
            {
                return new IType[] { NHibernateUtil.DateTimeOffset, TypeFactory.GetStringType(35) };
            }
        }

        /// <summary>
        /// Gets the type of the class returned by NullSafeGet().
        /// </summary>
        public Type ReturnedClass
        {
            get
            {
                return typeof(ZonedDateTime?);
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
            var zonedDateTime = AsZonedDateTime(component);
            if (zonedDateTime == null)
            {
                return null;
            }

            switch (property)
            {
                case 0:
                    return zonedDateTime.Value.ToDateTimeOffset();
                case 1:
                    return zonedDateTime.Value.Zone.Id;
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
            throw new InvalidOperationException("ZonedDateTime is an immutable object. SetPropertyValue isn't supported.");
        }

        public object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner)
        {
            var dateTimeOffset = (DateTimeOffset?)NHibernateUtil.DateTimeOffset.NullSafeGet(dr, names[0], session, owner);
            var timeZone = (string)NHibernateUtil.String.NullSafeGet(dr, names[1], session, owner);

            if (dateTimeOffset == null || timeZone == null)
            {
                return null;
            }

            return dateTimeOffset.ToZonedDateTime(DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZone));
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
        {
            if (value == null)
            {
                NHibernateUtil.DateTimeOffset.NullSafeSet(cmd, null, index);
                NHibernateUtil.String.NullSafeSet(cmd, null, index + 1);
            }
            else
            {
                var zonedDateTime = (ZonedDateTime)value;
                PropertyTypes[0].NullSafeSet(cmd, zonedDateTime.ToDateTimeOffset(), index, session);
                PropertyTypes[1].NullSafeSet(cmd, zonedDateTime.Zone.Id, index + 1, session);

            }
        }

        /// <summary>
        /// Compare two instances of the class mapped by this type for persistence
        /// "equality", i.e. equality of persistent state.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>true if the objects are considered equal; otherwise, false. If both x and y are null, the method returns true.</returns>
        public new bool Equals(object x, object y)
        {
            // Probably the best approach is to use object.Equals() which should call the overridden ZoneDateTime.Equals().
            // From the NodaTime api reference:
            //  http://nodatime.org/1.3.x/api/?topic=html/M_NodaTime_ZonedDateTime_Equals.htm
            // "True if the specified value is the same instant in the same time zone; false otherwise."
            // i.e. only true if the Instant and the DateTimeZone have the same values as those of the object being compared to.
            if (x == y)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }
            return x.Equals(y);

            #region Other equals option
            // The other option if we wanted to ignore the time zone and just compare on the Instant would be to use ZonedDateTime.CompareTo(object):
            // From the NodaTime api reference:
            //  http://nodatime.org/1.3.x/api/?topic=html/M_NodaTime_ZonedDateTime_CompareTo.htm)
            //  http://nodatime.org/1.3.x/api/?topic=html/P_NodaTime_ZonedDateTime_Comparer_Instant.htm
            // "A value less than zero if the instant represented by this zoned date/time is earlier than the one in other; 
            // zero if the instant is the same as the one in other; a value greater than zero if the instant is later than the one in other."
            ////var zonedDateTimeX = x as ZonedDateTime?;
            ////var zonedDateTimeY = y as ZonedDateTime?;
            ////if (zonedDateTimeX == null || zonedDateTimeY == null)
            ////{
            ////    return x.Equals(y);
            ////}

            ////if (zonedDateTimeX.Value.CompareTo(zonedDateTimeY.Value) == 0)
            ////{
            ////    return true;
            ////}

            ////return false;
            #endregion
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
        /// <param name="original">the value from the detached entity being merged.</param>
        /// <param name="target">the value in the managed entity.</param>
        /// <param name="session">The session.</param>
        /// <param name="owner">the managed entity.</param>
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

        /// <summary>
        /// Tries to cast an unknown object to an <see cref="ZonedDateTime"/> object.
        /// </summary>
        /// <param name="value">The object to try to cast to a <see cref="ZonedDateTime"/> object.</param>
        /// <returns>A <see cref="ZonedDateTime"/> object if the value can be cast.</returns>
        private static ZonedDateTime? AsZonedDateTime(object value)
        {
            if (value == null)
            {
                return null;
            }

            var ts = value as ZonedDateTime?;
            if (ts == null)
            {
                throw new InvalidCastException(
                    string.Format("Expected '{0}' but received '{1}'.", typeof(ZonedDateTime?), value.GetType()));
            }

            return ts;
        }
    }
}
