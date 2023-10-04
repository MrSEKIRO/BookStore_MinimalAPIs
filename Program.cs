using BookStore_MinimalAPIs.Context;
using BookStore_MinimalAPIs.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<BookStoreContext>(opt =>
	opt.UseInMemoryDatabase("BookStore"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: "MyAllowSpecificOrigins",
					  policy =>
					  {
						  policy.AllowAnyOrigin()
						  .AllowAnyHeader()
						  .AllowAnyMethod();
					  });
});

var app = builder.Build();

app.UseCors("MyAllowSpecificOrigins");

app.MapGet("/books", async (BookStoreContext db) =>
	await db.Books.ToListAsync());

app.MapGet("/books/{id}", async (int id, BookStoreContext db) =>
	await db.Books.FindAsync(id)
		is Book book
		? Results.Ok(book)
		: Results.NotFound());

app.MapPost("/books", async (Book book, BookStoreContext db) =>
{
	// check duplicate title
	if(await db.Books.AnyAsync(b => b.Title == book.Title))
	{
		return Results.BadRequest(new { Message = "Duplicate title" });
	}
	await db.Books.AddAsync(book);
	await db.SaveChangesAsync();
	return Results.Created($"/books/{book.Id}", book);
});

app.MapPut("/books/{id}", async (int id, Book book, BookStoreContext db) =>
{
	if(id != book.Id)
	{
		return Results.BadRequest();
	}

	// check duplicate title
	if(await db.Books.AnyAsync(b => b.Title == book.Title && b.Id != book.Id))
	{
		return Results.BadRequest(new { Message = "Duplicate title" });
	}

	db.Books.Update(book);
	await db.SaveChangesAsync();
	return Results.NoContent();
});

app.MapDelete("/books/{id}", async (int id, BookStoreContext db) =>
{
	var book = await db.Books.FindAsync(id);
	if(book == null)
	{
		return Results.NotFound();
	}
	db.Remove(book);
	await db.SaveChangesAsync();
	return Results.Ok(book);
});

app.Run();
