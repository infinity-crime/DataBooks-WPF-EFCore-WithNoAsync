using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBooks.Models.BookItems
{
    public class Tag
    {
        [Key]
        public string TagId { get; set; }

        // Relationships
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
