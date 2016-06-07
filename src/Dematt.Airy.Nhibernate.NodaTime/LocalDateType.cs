using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NodaTime;

namespace Dematt.Airy.Nhibernate.NodaTime
{
    public class LocalDateType : ImmutableUserType, IUserType
    {
        public SqlType[] SqlTypes
        {
            get
            {
                return new[] { SqlTypeFactory.Date };
            }
        }

        public Type ReturnedType
        {
            get { return typeof(LocalDate); }
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
            var value = NHibernateUtil.Date.NullSafeGet(rs, names);
            if (value == null)
            {
                return null;
            }
            var dateTime = (DateTime)value;
            return new LocalDate(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            var parm = (IDataParameter)cmd.Parameters[index];

            if (value == null)
            {
                parm.Value = DBNull.Value;
            }
            else
            {
                var localDate = (LocalDate)value;
                parm.DbType = DbType.Date;
                parm.Value = DateTime.SpecifyKind(new DateTime(localDate.Year, localDate.Month, localDate.Day), DateTimeKind.Local);
            }
        }
    }
}
