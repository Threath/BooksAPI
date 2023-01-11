using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BooksAPI.Models;

public class BooksDbContext : DbContext
{
    public BooksDbContext() { }

    public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options) { }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.FirstName).HasMaxLength(50);
            entity.Property(a => a.LastName).HasMaxLength(50);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(b => b.Id);

            entity.Property(b => b.Isbn).HasMaxLength(13).HasColumnName("ISBN");
            entity.Property(b => b.Rating).HasColumnType("decimal(18, 0)");
            entity.Property(b => b.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(ba => new { ba.BookId, ba.AuthorId });

            entity
                .HasOne<Book>(ba => ba.Book)
                .WithMany(ba => ba.BookAuthors)
                .HasForeignKey(ba => ba.BookId);
            entity
                .HasOne<Author>(ba => ba.Author)
                .WithMany(ba => ba.BookAuthors)
                .HasForeignKey(ba => ba.BookId);
        });
    }
}
