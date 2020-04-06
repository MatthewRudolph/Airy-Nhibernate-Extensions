using System;
using System.Data;
using System.Data.Common;
using NHibernate;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NodaTime;

namespace Dematt.Airy.Nhibernate.NodaTime
{
    public class DateTimeZoneTzdbType : ImmutableUserType, IUserType
    {
        public SqlType[] SqlTypes
        {
            get { return new SqlType[] { SqlTypeFactory.GetString(35) }; }
        }

        public Type ReturnedType
        {
            get { return typeof(DateTimeZone); }
        }

        public new bool Equals(object x, object y)
        {
            return object.Equals(x, y);
        }

        public int GetHashCode(object x)
        {
            return x == null ? 0 : x.GetHashCode();
        }

        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            var value = NHibernateUtil.String.NullSafeGet(rs, names, session);
            if (value == null)
            {
                return null;
            }

            return DateTimeZoneProviders.Tzdb.GetZoneOrNull((string) value);
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (value == null)
            {
                NHibernateUtil.String.NullSafeSet(cmd, null, index, session);
            }
            else
            {
                NHibernateUtil.String.NullSafeSet(cmd, ((DateTimeZone) value).Id, index, session);
            }
        }
    }
}