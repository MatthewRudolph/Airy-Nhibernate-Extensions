using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NodaTime;

namespace Dematt.Airy.Nhibernate.NodaTime
{
    public class OffsetDateTimeType : ImmutableUserType, IUserType
    {
        public SqlType[] SqlTypes
        {
            get { return new[] { SqlTypeFactory.DateTimeOffSet }; }
        }

        public Type ReturnedType
        {
            get { return typeof(OffsetDateTime); }
        }

        public new bool Equals(object x, object y)
        {
            if (x == null && y == null) return true;
            if (x == null || y == null) return false;
            return ((OffsetDateTime)x).ToInstant() == ((OffsetDateTime)y).ToInstant();
        }

        public int GetHashCode(object x)
        {
            return x == null ? 0 : ((OffsetDateTime)x).ToInstant().GetHashCode();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var value = NHibernateUtil.DateTimeOffset.NullSafeGet(rs, names);
            if (value == null)
            {
                return null;
            }
            return OffsetDateTime.FromDateTimeOffset((DateTimeOffset)value);
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null)
            {
                NHibernateUtil.DateTimeOffset.NullSafeSet(cmd, null, index);
            }
            else
            {
                NHibernateUtil.DateTimeOffset.NullSafeSet(cmd, ((OffsetDateTime)value).ToDateTimeOffset(), index);
            }
        }
    }
}
