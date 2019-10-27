using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web.DynamicData;
using Newtonsoft.Json;

namespace RobotBehaviourDesigner.Model
{
    public static class EntityObjectExtensions
    {

        public static string GetPrimaryKeyName(this object obj)
        {
            PropertyInfo propertyInfo = GetPropertyInfoByAttributeType(obj, typeof(KeyAttribute));
            return propertyInfo != null ? FindKeyName(propertyInfo) : "Uuid";
        }
		
        public static string GetDbColumnName(this PropertyInfo propertyInfo)
        {
            return propertyInfo != null ? FindKeyName(propertyInfo) : null;
        }

        public static string GetPrimaryKeyName<T>()
        {
            PropertyInfo propertyInfo = GetPropertyInfoByAttributeType<T>(typeof(KeyAttribute));
            return propertyInfo != null ? FindKeyName(propertyInfo) : "Uuid";
        }

        public static string GetPrimaryKeyTypeName<T>()
        {
            PropertyInfo propertyInfo = GetPropertyInfoByAttributeType<T>(typeof(KeyAttribute));
            return propertyInfo != null
                ? propertyInfo.PropertyType.Name
                : typeof(EntityBase).GetProperties().First(x => x.Name == "Uuid").PropertyType.Name;
        }

        public static void SetPrimaryKey(this object obj)
        {
            object key = GetPrimaryKeyValue(obj);
            if (key != null) return;
            PropertyInfo propertyType = obj.GetType().GetProperty(obj.GetPrimaryKeyName());
            propertyType?.SetValue(obj, Guid.NewGuid().ToString("N"), null);
        }

        public static string GetTableName<T>()
        {
            var tableNameAttribute = typeof(T).GetCustomAttributes(typeof(TableNameAttribute), true).FirstOrDefault() as TableNameAttribute;
            return tableNameAttribute?.Name;
        }

        public static object GetPrimaryKeyValue(this object obj)
        {
            return obj.GetType().GetProperty(obj.GetPrimaryKeyName())?.GetValue(obj);
        }

        public static string AddDoubleQuotes(this string value)
        {
            return "\"" + value + "\"";
        }

        private static PropertyInfo GetPropertyInfoByAttributeType(object item, Type attributeType)
        {
            return item.GetType().GetProperties().FirstOrDefault(x => Attribute.IsDefined(x, attributeType));
        }

        private static PropertyInfo GetPropertyInfoByAttributeType<T>(Type attributeType)
        {
            return typeof(T).GetProperties().FirstOrDefault(x => Attribute.IsDefined(x, attributeType));
        }

        private static string FindKeyName(MemberInfo propertyInfo)
        {
	        return (propertyInfo.GetCustomAttribute(typeof(JsonPropertyAttribute)) as JsonPropertyAttribute)?.PropertyName ?? propertyInfo.Name;
        }
    }
}