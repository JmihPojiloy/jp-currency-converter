using Microsoft.EntityFrameworkCore;

namespace debug.currency.converter.Data;

public class CurrencyDbContext: DbContext
{
    public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options)
        : base(options)
    {
        
    }
    
    public DbSet<CurrencyDto> BCurrencyRates { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CurrencyDto>()
            .HasNoKey();
    }
}