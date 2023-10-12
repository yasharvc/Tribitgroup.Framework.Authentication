using System.ComponentModel.DataAnnotations.Schema;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Tests.DbContextTest.DB
{
    [Table("Books")]
    internal class Book : Entity
    {
        protected int Age { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
