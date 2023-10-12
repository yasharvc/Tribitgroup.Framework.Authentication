using System.ComponentModel.DataAnnotations.Schema;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Tests.DbContextTest.DB
{
    [Table("Students")]
    internal class Student : Entity
    {
        public string FirstName { get; set; } = string.Empty;
    }
}
