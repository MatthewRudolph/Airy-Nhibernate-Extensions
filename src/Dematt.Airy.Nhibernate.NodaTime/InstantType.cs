using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NodaTime;

namespace Dematt.Airy.Nhibernate.NodaTime
{
    public class InstantType : IUserType
    {
        public SqlType[] SqlTypes
        {
            get { return new[] { SqlTypeFactory.Int64 }; }
        }

        public Type ReturnedType
        {
            get { return typeof(Instant); }
        }

        public bool IsMutable
        {
            get { return false; }
        }

        public new bool Equals(object x, object y)
        {
            return object.Equals(x, y);
        }

        public int GetHashCode(object x)
        {
            return x == null ? 0 : x.GetHashCode();
        }

        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            var value = NHibernateUtil.Int64.NullSafeGet(rs, names);
            if (value == null)
            {
                return null;
            }
            return new Instant((long)value);
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            if (value == null)
            {
                NHibernateUtil.Int64.NullSafeSet(cmd, null, index);
            }
            else
            {
                NHibernateUtil.Int64.NullSafeSet(cmd, ((Instant)value).Ticks, index);
            }
        }

        public object DeepCopy(object value)
        {
            return value;
        }

        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        public object Disassemble(object value)
        {
            return value;
        }
    }
}
