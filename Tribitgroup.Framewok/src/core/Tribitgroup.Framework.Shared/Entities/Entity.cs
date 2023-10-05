using Tribitgroup.Framework.Shared.Entities.Interfaces;
using Tribitgroup.Framework.Shared.Extensions;

namespace Tribitgroup.Framework.Shared.Entities
{
    public class Entity<T> : IEntity<T> where T: notnull
    {
        public T Id { get; protected init; }
    }

    public class Entity:Entity<Guid> 
    {
        public Entity()
        {
            Id = BasicTypesExtensions.GetSequentialGuid();
        }
    }

    public record EntityRecord(Guid Id) : IEntity<Guid>
    {
        public EntityRecord() : this(BasicTypesExtensions.GetSequentialGuid()) { }
    }
}