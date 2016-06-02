using System;
using System.Data;
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
                return new[] { "DateTimeWithOffset", "TimeZone" };
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

        public int GetHashCode(object x)
        {
            throw new System.NotImplementedException();
        }

        public bool IsMutable { get; private set; }

        public object DeepCopy(object value)
        {
            throw new System.NotImplementedException();
        }

        public object Disassemble(object value, ISessionImplementor session)
        {
            throw new System.NotImplementedException();
        }

        public object Assemble(object cached, ISessionImplementor session, object owner)
        {
            throw new System.NotImplementedException();
        }

        public object Replace(object original, object target, ISessionImplementor session, object owner)
        {
            throw new System.NotImplementedException();
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index, bool[] settable, ISessionImplementor session)
        {
            throw new System.NotImplementedException();
        }

        public object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner)
        {
            throw new System.NotImplementedException();
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
