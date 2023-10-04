using BookStore_MinimalAPIs.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore_MinimalAPIs.Context
{
	public class BookStoreContext : DbContext
	{
		public BookStoreContext(DbContextOptions<BookStoreContext> options) : base(options)
		{
		}

		public DbSet<Book> Books { get; set; }
	}
}
