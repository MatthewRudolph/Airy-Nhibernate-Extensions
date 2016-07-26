using System;
using System.Data;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Type;
using NHibernate.UserTypes;
using NodaMoney;

namespace Dematt.Airy.Nhibernate.NodaMoney
{
    public class MoneyType : ImmutableCompositeUserType, ICompositeUserType
    {
        /// <summary>
        /// Gets the "property names" that may be used in a query.
        /// </summary>
        public string[] PropertyNames
        {
            get
            {
                return new[] { "Amount", "Currency" };
            }
        }

        /// <summary>
        /// Gets the corresponding "property types".
        /// </summary>
        public IType[] PropertyTypes
        {
            get
            {
                return new IType[] { NHibernateUtil.Decimal, TypeFactory.GetStringType(3) };
            }
        }

        /// <summary>
        /// Gets the type of the class returned by NullSafeGet().
        /// </summary>
        public Type ReturnedClass
        {
            get
            {
                return typeof(Money?);
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
            // The best approach is to use object.Equals() which should call the overridden Money.Equals().
            // Which implements all/any required the logic for us.
            if (x == null && y == null)
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

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
        /// Gets the value of a property.
        /// </summary>
        /// <param name="component">An instance of the class mapped by this "type".</param>
        /// <param name="property">The index of the property to get.</param>
        /// <returns>The property value.</returns>
        public object GetPropertyValue(object component, int property)
        {
            var money = AsMoney(component);
            if (money == null)
            {
                return null;
            }

            switch (property)
            {
                case 0:
                    return money.Value.Amount;
                case 1:
                    return money.Value.Currency.Code;
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
            throw new InvalidOperationException("Money is an immutable object. SetPropertyValue isn't supported.");
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
                NHibernateUtil.Decimal.NullSafeSet(cmd, null, index);
                NHibernateUtil.String.NullSafeSet(cmd, null, index + 1);
            }
            else
            {
                var money = (Money)value;
                PropertyTypes[0].NullSafeSet(cmd, money.Amount, index, session);
                PropertyTypes[1].NullSafeSet(cmd, money.Currency.Code, index + 1, session);

            }
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
            var amount = (Decimal?)NHibernateUtil.Decimal.NullSafeGet(dr, names[0], session, owner);
            var currency = (string)NHibernateUtil.String.NullSafeGet(dr, names[1], session, owner);

            if (amount == null || currency == null)
            {
                return null;
            }

            return new Money(amount.Value, Currency.FromCode(currency));
        }

        /// <summary>
        /// Tries to cast an unknown object to a <see cref="Money"/> object.
        /// </summary>
        /// <param name="value">The object to try to cast to a <see cref="Money"/> object.</param>
        /// <returns>A <see cref="Money"/> object if the value can be cast.</returns>
        private static Money? AsMoney(object value)
        {
            if (value == null)
            {
                return null;
            }

            var ts = value as Money?;
            if (ts == null)
            {
                throw new InvalidCastException(
                    string.Format("Expected '{0}' but received '{1}'.", typeof(Money?), value.GetType()));
            }

            return ts;
        }
    }
}
