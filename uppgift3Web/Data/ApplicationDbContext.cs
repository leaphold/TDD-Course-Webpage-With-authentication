using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using uppgift3Web.Models;

namespace uppgift3Web.Data
{
public interface IApplicationDbContext
{
    DbSet<Product> Products { get; set; }
    DbSet<Category> Categories { get; set; }
    int SaveChanges();
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class ApplicationDbContext : IdentityDbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(ConfigureProduct);
            modelBuilder.Entity<Category>(ConfigureCategory);

            SeedData(modelBuilder);
        }

        private void ConfigureProduct(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Price)
                .IsRequired()
                .HasColumnType("decimal(18, 2)"); // Specify the SQL Server column type

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);
        }

        private void ConfigureCategory(EntityTypeBuilder<Category> builder)
        {
            // Additional configurations for the Category entity if needed
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Frukter"
                },
                new Category
                {
                    Id = 2,
                    Name = "Grönsaker"
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Banan",
                    Description = "En gul frukt",
                    Price = 10,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 2,
                    Name = "Äpple",
                    Description = "En röd frukt",
                    Price = 5,
                    CategoryId = 1
                },
                new Product
                {
                    Id = 3,
                    Name = "Päron",
                    Description = "En grön frukt",
                    Price = 15,
                    CategoryId = 1
                },
                new Product 
                {
                    Id = 4,
                    Name = "Gurka",
                    Description = "En grön grönsak",
                    Price = 20, 
                    CategoryId = 2
                }
            );
        }
    }
}
