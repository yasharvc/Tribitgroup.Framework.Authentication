using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Dapper.Tests
{
    public class Order : AggregateRoot
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
