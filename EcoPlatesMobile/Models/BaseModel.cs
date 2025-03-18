
namespace EcoPlatesMobile.Models
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    public abstract class BaseModel
    {
        protected Dictionary<string, object> PropertyList = [];

        protected BaseModel()
        {
            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                if (!PropertyList.ContainsKey(prop.Name))
                {
                    object defaultValue = GetSafeDefault(prop.PropertyType);
                    PropertyList[prop.Name] = defaultValue;
                }
            }
        }

        protected T GetValue<T>([CallerMemberName] string propertyName = "")
        {
            if (!PropertyList.ContainsKey(propertyName))
                return GetSafeDefault<T>();

            return PropertyList[propertyName] is T value ? value : GetSafeDefault<T>();
        }

        protected void SetValue<T>(T value, [CallerMemberName] string propertyName = "")
        {
            if (value == null)
                throw new ArgumentNullException(propertyName, "Property value cannot be null.");

            if (PropertyList.ContainsKey(propertyName))
            {
                if (EqualityComparer<T>.Default.Equals((T)PropertyList[propertyName], value)) return;
                PropertyList[propertyName] = value;
            }
            else
            {
                PropertyList.Add(propertyName, value);
            }
        }

        private static object GetSafeDefault(Type type)
        {
            if (type == typeof(string)) return string.Empty;
            if (type.IsValueType) return Activator.CreateInstance(type)!;
            return Activator.CreateInstance(type) ?? new object();
        }

        private static T GetSafeDefault<T>()
        {
            if (typeof(T) == typeof(string)) return (T)(object)string.Empty;
            if (typeof(T).IsValueType) return Activator.CreateInstance<T>();
            return default!;
        }
    }

}
