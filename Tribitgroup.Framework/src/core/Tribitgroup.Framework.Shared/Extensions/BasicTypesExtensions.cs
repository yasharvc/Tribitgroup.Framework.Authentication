using SequentialGuid;

namespace Tribitgroup.Framework.Shared.Extensions
{
    public static class BasicTypesExtensions
    {
        public static Guid GetSequentialGuid()
        {
            return SequentialSqlGuidGenerator.Instance.NewGuid();
        }
    }
}