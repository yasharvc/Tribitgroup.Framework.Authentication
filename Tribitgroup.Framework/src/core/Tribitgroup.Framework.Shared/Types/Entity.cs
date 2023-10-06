using System.ComponentModel.DataAnnotations.Schema;
using Tribitgroup.Framework.Shared.Extensions;
using Tribitgroup.Framework.Shared.Interfaces;

namespace Tribitgroup.Framework.Shared.Types
{
    public class Entity : Entity<Guid>
    {
        public Entity()
        {
            Id = BasicTypesExtensions.GetSequentialGuid();
        }
    }
    public class Entity<T> : IEntity<T> where T : notnull
    {
        IDictionary<Type,string> TableNames { get; set; } = new Dictionary<Type,string>();
        public T Id { get; set; }
        public string GetTableName()
        {
            var type = GetType();
            if (TableNames.TryGetValue(type, out string? value))
                return value;
            dynamic? tableAttr = GetType().GetCustomAttributes(false).SingleOrDefault(attr => attr.GetType().Name == nameof(TableAttribute));
            var res = TableNames[type] = tableAttr is null ? GetType().Name : tableAttr.Name;
            return res;
        }
    }
}