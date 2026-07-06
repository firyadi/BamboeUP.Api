using System;
using System.Reflection;

namespace Repository.Extensions
{
    internal static class AuditTimestampHelper
    {
        public static void StampCreate(object entity)
        {
            SetDateTime(entity, "CreatedTime", DateTime.UtcNow);
        }

        public static void StampUpdate(object entity)
        {
            SetDateTime(entity, "UpdatedTime", DateTime.UtcNow);
        }

        public static void StampSoftDelete(object entity, long deletedBy)
        {
            SetLong(entity, "DeletedById", deletedBy);
            SetDateTime(entity, "DeletedTime", DateTime.UtcNow);
        }

        private static void SetDateTime(object entity, string propertyName, DateTime value)
        {
            var property = entity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property is null || !property.CanWrite)
                return;

            if (property.PropertyType == typeof(DateTime))
            {
                property.SetValue(entity, value);
                return;
            }

            if (property.PropertyType == typeof(DateTime?))
                property.SetValue(entity, value);
        }

        private static void SetLong(object entity, string propertyName, long value)
        {
            var property = entity.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property is null || !property.CanWrite)
                return;

            if (property.PropertyType == typeof(long))
            {
                property.SetValue(entity, value);
                return;
            }

            if (property.PropertyType == typeof(long?))
                property.SetValue(entity, value);
        }
    }
}
