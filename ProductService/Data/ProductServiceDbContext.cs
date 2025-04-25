using Microsoft.EntityFrameworkCore;
using ProductService.Domain;

namespace ProductService.Data;

public class ProductServiceDbContext(DbContextOptions<ProductServiceDbContext> options) : DbContext(options)
{
    public virtual DbSet<Product> Products => Set<Product>();
    public virtual DbSet<Category> Categories => Set<Category>();
}