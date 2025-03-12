using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBooks.Models.BookItems;
using Microsoft.EntityFrameworkCore;

namespace DataBooks.Models.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        public AppDbContext() : base() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=C:\\Users\\User\\source\\repos\\DataBooks\\bin\\Debug\\net9.0-windows\\dataBooksMain.db");
        }
    }
}
