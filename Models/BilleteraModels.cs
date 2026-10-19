using Microsoft.EntityFrameworkCore;

namespace ApibilleteraVirtual.Models;

public class Cuenta
{
    public int Id { get; set; }
    public string NombreTitular { get; set; } = "";
    public string NumeroDocumento { get; set; } = "";
    public decimal Saldo { get; set; }
    public List<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}

public class Movimiento
{
    public int Id { get; set; }
    public int CuentaId { get; set; }
    public string Tipo { get; set; } = "";
    public decimal Monto { get; set; }
    public string Descripcion { get; set; } = "";
    public DateTime Fecha { get; set; } = DateTime.Now;
}

public class CrearCuentaRequest
{
    public string NombreTitular { get; set; } = "";
    public string NumeroDocumento { get; set; } = "";
    public decimal SaldoInicial { get; set; }
}

public class OperacionMontoRequest
{
    public decimal Monto { get; set; }
    public string Descripcion { get; set; } = "";
}
public class Extracto
{
    public int CuentaId { get; set; }
    public string Titular { get; set; } = "";
    public decimal SaldoFinal { get; set; }
    public List<Movimiento> Historial { get; set; } = new List<Movimiento>();
}

public class TransferenciaRequest
{
    public int CuentaOrigenId { get; set; }
    public int CuentaDestinoId { get; set; }
    public decimal Monto { get; set; }
}

public class BilleteraDbContext : DbContext
{
    public BilleteraDbContext(DbContextOptions<BilleteraDbContext> options) : base(options)
    {
    }

    public DbSet<Cuenta> Cuentas { get; set; }
    public DbSet<Movimiento> Movimientos { get; set; }
}