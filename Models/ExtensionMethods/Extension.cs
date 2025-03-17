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
    public enum OrderByOptions
    {
        ByVotes,
        ByPriceLowestFirst,
        ByPublicationDate
    }

    public static class Extension
    {
        public static IQueryable<BookListDto> MapBookToDto(this IQueryable<Book> books)
        {
            return books.Select(book => new BookListDto
            {
                BookId = book.BookId,
                Price = book.Price,
                Title = book.Title,
                PublishedOn = book.PublishedOn,
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

        public static IQueryable<BookListDto> OrderBooksBy(this IQueryable<BookListDto> books, 
            OrderByOptions options)
        {
            switch(options)
            {
                case OrderByOptions.ByVotes:
                    return books.OrderByDescending(b => b.ReviewsAverageVotes);

                case OrderByOptions.ByPublicationDate:
                    return books.OrderByDescending(b => b.PublishedOn);

                case OrderByOptions.ByPriceLowestFirst:
                    return books.OrderBy(b => (double)b.Price);

                default:
                    throw new ArgumentOutOfRangeException(nameof(options), options, null);
            }
        }
    }
}
