using DataBooks.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DataBooks.Models.BookItems;

namespace DataBooks.Models.ExtensionMethods
{
    public static class Extension
    {
        public static IQueryable<BookListDto> MapBookToDto(this IQueryable<Book> books)
        {
            return books.Select(book => new BookListDto
            {
                BookId = book.BookId,
                Title = book.Title,
                PublishedOn = book.PublishedOn.ToString("dd-MM-yyyy"),
                Publisher = book.Publisher,

                Author = string.Join(", ",
                book.AuthorsLink
                .Select(a => a.Name)),

                ReviewsCount = book.Reviews.Count,
                ReviewComment = book.Reviews.Count != 0 ?
                string.Join("; ", book.Reviews.Select(r => r.Comment))
                : "Пусто",

                ReviewsAverageVotes = book.Reviews.Count != 0 ?
                book.Reviews.Select(r => (double)r.NumStars).Average()
                : (double)0,

                TagStrings = string.Join(", ", 
                book.Tags.Select(t => t.TagId))
            });
        }
    }
}
