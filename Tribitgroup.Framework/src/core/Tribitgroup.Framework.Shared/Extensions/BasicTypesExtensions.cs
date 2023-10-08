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
    }
}