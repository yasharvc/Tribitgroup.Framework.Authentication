using SequentialGuid;
using System.Collections;

namespace Tribitgroup.Framework.Shared.Extensions
{
    public static class BasicTypesExtensions
    {
        public static Guid GetSequentialGuid()
        {
            return SequentialSqlGuidGenerator.Instance.NewGuid();
        }

        public static bool IsArrayOrList(this Type type)
        {
            return type.IsArray || (type.IsGenericType && type.GetInterfaces().Contains(typeof(IEnumerable)));
        }

        public static bool IsArrayOrList<T>(this T instance)
        {
            var type= typeof(T);
            return type.IsArray || (type.IsGenericType && type.GetInterfaces().Contains(typeof(IEnumerable)));
        }

        public static bool IsBasicType(this Type type)
        {
            return
                type == typeof(int) ||
                type == typeof(double) ||
                type == typeof(char) ||
                type == typeof(bool) ||
                type == typeof(string) ||
                type == typeof(byte) ||
                type == typeof(short) ||
                type == typeof(long) ||
                type == typeof(float) ||
                type == typeof(decimal) ||
                type == typeof(Guid) ||
                type == typeof(DateTime) ||
                type == typeof(sbyte) ||
                type == typeof(ushort) ||
                type == typeof(uint);
        }

        public static object? ChangeTo<T>(this T instance, Type toType, object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return default;
            }

            if (toType.IsEnum)
            {
                return Enum.Parse(toType, value.ToString() ?? "");
            }

            if (toType == typeof(Guid))
            {
                return new Guid(value.ToString() ?? "");
            }

            if (toType == typeof(DateTime))
            {
                if (DateTime.TryParse(value.ToString(), out DateTime dateTime))
                {
                    return dateTime;
                }
                return default;
            }

            try
            {
                return Convert.ChangeType(value, toType);
            }
            catch
            {
                return default(T);
            }
        }

        public static T? To<T>(this object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return default;
            }

            if (typeof(T).IsEnum)
            {
                return (T)Enum.Parse(typeof(T), value.ToString() ?? "");
            }

            if (typeof(T) == typeof(Guid))
            {
                return (T)(object)new Guid(value.ToString() ?? "");
            }

            if (typeof(T) == typeof(DateTime))
            {
                if (DateTime.TryParse(value.ToString(), out DateTime dateTime))
                {
                    return (T)(object)dateTime;
                }
                return default;
            }

            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        public static Type? GetGenericTypeFromList(this Type type, string propName)
            => type.GetProperty(propName)?.PropertyType.GenericTypeArguments[0];
    }
}