using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBooks.Models.BookItems
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string Name { get; set; }

        // Relationships
        public ICollection<Book> BooksLink { get; set; } = new List<Book>(); 
    }
}
