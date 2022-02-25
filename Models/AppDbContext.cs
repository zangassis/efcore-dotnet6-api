using BlogManager.Models;
using Microsoft.EntityFrameworkCore;

namespace BlogManager.Data;

public class AppDbContext : DbContext
{
    public DbSet<BlogPost>? Posts { get; set; }

    public DbSet<Tag>? Tags { get; set; }

    public DbSet<Author>? Authors { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("DataSource = blogdb.db; Cache=Shared");
}
