using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
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

        public string GetTableName(DbContext context)
        {
            var type = GetType();
            if (TableNames.TryGetValue(type, out string? value))
                return value;
            dynamic? tableAttr = GetType().GetCustomAttributes(false).SingleOrDefault(attr => attr.GetType().Name == nameof(TableAttribute));
            var dbSetProperties = context.GetType()
                .GetProperties()
                .Where(prop => prop.PropertyType.IsGenericType &&
                               prop.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                               type == prop.PropertyType.GenericTypeArguments.First());

            var dbSetProperty = dbSetProperties.SingleOrDefault() ?? throw new EntryPointNotFoundException();
            var res = TableNames[type] = tableAttr is null ? dbSetProperty.Name : tableAttr.Name;
            return res;
        }
    }
}