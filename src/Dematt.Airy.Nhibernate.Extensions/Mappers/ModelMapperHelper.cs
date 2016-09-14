using System;
using Dematt.Airy.Nhibernate.Extensions.UserTypes;
using NHibernate.Mapping.ByCode;

namespace Dematt.Airy.Nhibernate.Extensions.Mappers
{
    /// <summary>
    /// Provides helper methods for setting NHibernate ModelMappers.
    /// </summary>
    public static class ModelMapperHelper
    {
        /// <summary>
        /// Applies the <see cref="DateTimeOffsetSplitType"/> Composite user type to all <see cref="DateTimeOffset"/> fields in the mapping.
        /// </summary>
        /// <remarks>
        /// Allows the use of <see cref="DateTimeOffset"/> type with databases that do not natively support it.
        /// User: mapper.BeforeMapProperty += ModelMapperHelper.ApplyDateTimeOffsetSplitTypeToDateTimeOffset
        /// </remarks>
        public static void ApplyDateTimeOffsetSplitTypeToDateTimeOffset(IModelInspector inspector, PropertyPath property, IPropertyMapper mapper)
        {
            Type propertyType = property.LocalMember.GetPropertyOrFieldType();
            if (propertyType == typeof(DateTimeOffset) || propertyType == typeof(DateTimeOffset?))
            {
                mapper.Type(typeof(DateTimeOffsetSplitType), null);
                string columName = property.ToColumnName();
                mapper.Columns(n => n.Name(columName + "DateTime"), n => n.Name(columName + "Offset"));
            }
        }

        /// <summary>
        /// Instructs the mapper to set all Abstract classes as root entities.
        /// </summary>
        /// <remarks>
        /// mapper.IsRootEntity(ModelMapperHelper.ApplyAbstractClassesAsRootEntites);
        /// </remarks>
        public static bool ApplyAbstractClassesAsRootEntites(Type type, bool declared)
        {
            return type.IsAbstract == false;
        }
    }
}
