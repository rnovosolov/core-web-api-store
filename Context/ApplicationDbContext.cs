using CoreWebAPIstore.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options) { }
    

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }


    //this for many-to-many relationships
    //public DbSet<ProductCategory> ProductsCategories { get; set; }

    /*protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .HasColumnType(pc => new {pc.ProductId, pc.CategoryId )
    .Property(object => object.property).HasPrecision(12, 10);
    }*/

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        optionsBuilder.UseSqlServer(connectionString);
        //base.OnConfiguring(optionsBuilder);
    }
}