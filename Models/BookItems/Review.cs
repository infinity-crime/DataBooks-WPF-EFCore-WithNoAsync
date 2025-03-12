using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBooks.Models.BookItems
{
    public class Review
    {
        public int ReviewId { get; set; }
        public int NumStars { get; set; }
        public string Comment { get; set; }

        // Relationships
        public int BookId { get; set; }
    }
}
