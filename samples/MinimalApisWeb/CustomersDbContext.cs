using Microsoft.EntityFrameworkCore;
using MinimalApisWeb.Models;

namespace MinimalApisWeb;

public class CustomersDbContext : DbContext
{
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options)
    {
        
    }
    public DbSet<Customer> Customers { get; set; } = null!;
}