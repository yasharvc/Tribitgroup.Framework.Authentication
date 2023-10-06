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
        IDictionary<string,string> TableNames { get; set; } = new Dictionary<string,string>();
        public T Id { get; set; }
        public string GetTableName()
        {
            var name = GetType().FullName ?? "";
            if (TableNames.TryGetValue(name, out string? value))
                return value;
            dynamic? tableAttr = GetType().GetCustomAttributes(false).SingleOrDefault(attr => attr.GetType().Name == nameof(TableAttribute));
            var res = TableNames[name] = tableAttr is null ? GetType().Name : tableAttr.Name;
            return res;
        }
    }
}