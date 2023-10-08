using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tribitgroup.Framework.Identity.Tests.GeneralTests
{
    class Order
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    class OrderItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }

    public class JsonConvertTests
    {
        [Fact]
        public void TestJsonConvert()
        {
            var obj = new
            {
                Id = Guid.NewGuid(),
                Name = "test",
                OrderItems = new object[]
                {
                    new {Id = Guid.NewGuid(), Name = "object 1", Count = 3, Price = 120 },
                    new {Id = Guid.NewGuid(), Name = "object 2", Count = 5, Price = 500 },
                }
            };

            var res = JsonConvert.DeserializeObject<Order>(JsonConvert.SerializeObject(obj));

            Assert.NotEmpty(res.OrderItems);
        }
    }
}
