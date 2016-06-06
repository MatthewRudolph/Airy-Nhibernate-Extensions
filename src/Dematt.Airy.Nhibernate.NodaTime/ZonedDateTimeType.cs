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
    /// <summary>
    /// Abstract class the implements all of the methods required to store a <see cref="ZonedDateTime"/> except,
    /// the NullSafeGet method which is implemented in two derived class one for Tzdb data sources and the other for
    /// </summary>
    /// <remarks>
    /// This could have also be implemented using the <see cref="IParameterizedType"/> interface.
    /// However it is currently preferred to have this base class and then two implantations of it.
    /// </remarks>
    public abstract class ZonedDateTimeType : ImmutableCompositeUserType
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
