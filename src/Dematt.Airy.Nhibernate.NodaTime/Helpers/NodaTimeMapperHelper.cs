using System;
using System.Diagnostics;
using NHibernate.Mapping.ByCode;
using NodaTime;

namespace Dematt.Airy.Nhibernate.NodaTime.Helpers
{
    /// <summary>
    /// Provides helper methods for setting up NHibernate ModelMapper classes to map NodaTime types.
    /// </summary>
    /// <remarks>THIS DOES NOT WORK AS IT TRIES TO MAP THE CLASSES THAT MAKE UP OFFSETDATETIME RATHER THAN OFFSETDATETIME ITSELF.</remarks>
    [Obsolete]
    public static class NodaTimeMapperHelper
    {
        /// <summary>
        /// Applies the <see cref="OffsetDateTimeType"/> UserType to all <see cref="OffsetDateTime"/> fields in the mapping.
        /// </summary>
        /// <remarks>
        /// Allows the use of <see cref="OffsetDateTime"/> type with databases that do not natively support it.
        /// User: mapper.BeforeMapProperty += NodaTimeMapperHelper.ApplyOffsetDateTimeType
        /// </remarks>
        public static void ApplyOffsetDateTimeType(IModelInspector inspector, PropertyPath property, IPropertyMapper mapper)
        {
            var root = property.GetRootMember();
            Debug.WriteLine(root.Name);
            Type propertyType = property.LocalMember.GetPropertyOrFieldType();
            if (propertyType == typeof(OffsetDateTime) || propertyType == typeof(OffsetDateTime?))
            {
                mapper.Type<OffsetDateTimeType>();
            }
        }
    }
}
