using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBooks.Models.DTO
{
    public class BookListDto
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string PublishedOn { get; set; }
        public string Publisher { get; set; }
        public string Author { get; set; }
        public int ReviewsCount { get; set; } // кол-во отзывов на книгу
        public string ReviewComment { get; set; }
        public double ReviewsAverageVotes { get; set; } // средняя оценка книги
        public string TagStrings { get; set; }

        public bool HasReviews => ReviewsCount > 0; // для Visibility
    }
}
