using System;
using Tribitgroup.Framework.Shared.Types;

namespace Tribitgroup.Framework.Identity.Tests.DbContextTest.DB
{
    internal class ISBN : Entity
    {
        public string Code { get; set; } = string.Empty;
        public Guid BookId { get; set; }
    }
}
