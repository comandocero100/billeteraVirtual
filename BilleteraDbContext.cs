using ApibilleteraVirtual.Models;
using Microsoft.EntityFrameworkCore;

public class BilleteraDbContext : DbContext
{
    public BilleteraDbContext(DbContextOptions<BilleteraDbContext> options) : base(options) { }

    public DbSet<Cuenta> Cuentas => Set<Cuenta>();
    public DbSet<Movimiento> Movimientos => Set<Movimiento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cuenta>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Cuenta>()
            .HasMany(c => c.Movimientos)
            .WithOne()
            .HasForeignKey(m => m.CuentaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}