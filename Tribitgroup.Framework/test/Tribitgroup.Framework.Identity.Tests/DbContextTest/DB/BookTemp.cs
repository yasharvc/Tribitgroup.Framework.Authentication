using System.ComponentModel.DataAnnotations.Schema;

namespace Tribitgroup.Framework.Identity.Tests.DbContextTest.DB
{
    [Table("BooksTemp")]
    internal class BookTemp : Book
    {
        public bool IsTemp { get; set; } = false;
        //public static implicit operator BookTemp(Book b)
        //{
        //    return new BookTemp { Name = b.Name, Id = b.Id };
        //}
    }
}
