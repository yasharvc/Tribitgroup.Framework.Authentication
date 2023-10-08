using System;
using System.ComponentModel.DataAnnotations.Schema;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Tests.DbContextTest.DB
{
    [Table("Borrows")]
    internal class Borrow : Entity
    {
        public Guid StudentId { get; set; }
        public Guid BookId { get; set; }
    }
}
