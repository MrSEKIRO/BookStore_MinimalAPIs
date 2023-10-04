using System.Reflection.Metadata.Ecma335;

namespace BookStore_MinimalAPIs.Models
{
	public class Book
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
	}
}
