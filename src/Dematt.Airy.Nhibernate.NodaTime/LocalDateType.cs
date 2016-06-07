using System;
using System.Data;
using NHibernate;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using NodaTime;

namespace Dematt.Airy.Nhibernate.NodaTime
{
    /// <summary>
    /// An NHibernate Custom User Type for the NodaTime <see cref="LocalDate"/> struct.    
    /// </summary>
    /// <remarks>
    /// This allows the LocalDate struct to be used as properties in the domain and mapped using Nhibernate.
    /// It will be store it in the database in a single field with a data type of date.
    /// </remarks>
    public class LocalDateType : ImmutableUserType, IUserType
    {
        /// <summary>
        /// The SqlType used to store the data in the database.
        /// </summary>
        /// <remarks>
        /// The <see cref="T:NHibernate.Driver.IDriver"/> uses the SqlType to get enough
        /// information to create an <see cref="T:System.Data.IDbDataParameter"/>.
        /// The <see cref="T:NHibernate.Dialect.Dialect"/> uses the SqlType to convert 
        /// the <see cref="P:NHibernate.SqlTypes.SqlType.DbType"/>to the appropriate sql type for SchemaExport.
        /// </remarks>
        public SqlType[] SqlTypes
        {
            get
            {
                return new[] { SqlTypeFactory.Date };
            }
        }

        /// <summary>
        /// Gets the type of the class returned by NullSafeGet().
        /// </summary>
        public Type ReturnedType
        {
            get { return typeof(LocalDate); }
        }

        /// <summary>
        /// Retrieve an instance of the mapped class from a ADO.Net result set.
        /// Implementers should handle possibility of null values.
        /// </summary>
        /// <param name="rs">An IDataReader.</param>
        /// <param name="names">The column names.</param>
        /// <param name="owner">The containing entity.</param>        
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

        /// <summary>
        /// Write an instance of the mapped class to a prepared statement. We should handle possibility of null values.
        /// A multi-column type should be written to parameters starting from index.
        /// If a property is not settable, skip it and don't increment the index.
        /// </summary>
        /// <param name="cmd">The command used for writing the value.</param>
        /// <param name="value">The value to write.</param>
        /// <param name="index">The parameters index to start at.</param>
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

        /// <summary>
        /// Compare two instances of the class mapped by this type for persistence
        /// "equality", i.e. equality of persistent state.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>        
        public new bool Equals(object x, object y)
        {
            return object.Equals(x, y);
        }

        /// <summary>
        /// Gets a hash code for the instance, consistent with persistence "equality"
        /// </summary>
        /// <param name="x">The object to get the hash code for.</param>
        public int GetHashCode(object x)
        {
            return x == null ? 0 : x.GetHashCode();
        }
    }
}
