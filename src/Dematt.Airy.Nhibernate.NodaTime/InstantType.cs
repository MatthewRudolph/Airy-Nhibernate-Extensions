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
    public class InstantType : ImmutableUserType, IUserType
    {
        public SqlType[] SqlTypes
        {
            get { return new[] { SqlTypeFactory.Int64 }; }
        }

        public Type ReturnedType
        {
            get { return typeof(Instant); }
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
            var value = NHibernateUtil.Int64.NullSafeGet(rs, names, session);
            if (value == null)
            {
                return null;
            }
            return Instant.FromUnixTimeTicks((long) value);
        }

        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (value == null)
            {
                NHibernateUtil.Int64.NullSafeSet(cmd, null, index, session);
            }
            else
            {
                NHibernateUtil.Int64.NullSafeSet(cmd, ((Instant)value).ToUnixTimeTicks(), index, session);
            }
        }
    }
}
