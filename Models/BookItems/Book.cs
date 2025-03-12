using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DataBooks.Models.BookItems
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedOn { get; set; }
        public string Publisher { get; set; }
        public decimal Price { get; set; }

        // Relationships
        public ICollection<Author> AuthorsLink { get; set; } = new List<Author>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
    }
}
