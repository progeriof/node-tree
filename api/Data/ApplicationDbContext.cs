using Microsoft.EntityFrameworkCore;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    // protected override void OnModelCreating(ModelBuilder modelBuilder)
    // {
    //     modelBuilder.Entity<Node>()
    //         .HasMany(e => e.children);
    // }

    public DbSet<Journal> Journals { get; set; }
    public DbSet<Node> Nodes { get; set; }
}