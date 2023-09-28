using SequentialGuid;

namespace Tribitgroup.Framewok.Shared.Extensions
{
    public static class BasicTypesExtensions
    {
        public static Guid GetSequentialGuid()
        {
            return SequentialSqlGuidGenerator.Instance.NewGuid();
        }
    }
}