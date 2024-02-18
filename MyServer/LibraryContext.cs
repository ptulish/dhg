using Microsoft.EntityFrameworkCore;

namespace MyServer;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
    {
        
    }
    
    public DbSet<Book> Books { get; set; }
}