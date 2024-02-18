using Microsoft.EntityFrameworkCore;

namespace Library;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options)
    {
        
    }
    
    public DbSet<Book> Books { get; set; }
}