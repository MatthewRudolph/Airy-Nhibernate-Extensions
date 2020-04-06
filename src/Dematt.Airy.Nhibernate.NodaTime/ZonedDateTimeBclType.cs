using System;
using System.Data;
using Dematt.Airy.Nhibernate.NodaTime.Extensions;
using NHibernate;
using NHibernate.Engine;
using NHibernate.UserTypes;
using NodaTime;

namespace Dematt.Airy.Nhibernate.NodaTime
{
    /// <summary>
    /// An NHibernate Composite User Type for the NodaTime <see cref="ZonedDateTime"/> struct.
    /// The DateTimeZones provider used is DateTimeZoneProviders.Bcl
    /// </summary>
    /// <remarks>
    /// This allows the ZonedDateTime struct to be used as properties in the domain and mapped using Nhibernate.
    /// It will be store it in the database as two separate columns a DateTimeOffset for the Instant and a string for the DateTimeZone.
    /// This implementation expects the DateTimeZones provider to be DateTimeZoneProviders.Bcl
    /// </remarks>
    public class ZonedDateTimeBclType : ZonedDateTimeType, ICompositeUserType
    {
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
            var dateTimeOffset = (DateTimeOffset?)NHibernateUtil.DateTimeOffset.NullSafeGet(dr, names[0], session, owner);
            var timeZone = (string)NHibernateUtil.String.NullSafeGet(dr, names[1], session, owner);

            if (dateTimeOffset == null || timeZone == null)
            {
                return null;
            }

            return dateTimeOffset.ToZonedDateTime(DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZone));
        }
    }
}
