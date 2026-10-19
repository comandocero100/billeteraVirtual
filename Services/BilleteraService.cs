using Microsoft.EntityFrameworkCore;
using ApibilleteraVirtual.Models;

namespace ApibilleteraVirtual.Services;

public interface IBilleteraService
{
    Task<List<Cuenta>> ObtenerCuentas();
    Task<Cuenta?> ObtenerCuenta(int id);
    Task<string> CrearCuenta(CrearCuentaRequest req);
    Task<string> Depositar(int id, decimal monto, string desc);
    Task<string> Retirar(int id, decimal monto, string desc);
    Task<string> Transferir(TransferenciaRequest req);
}

public class BilleteraService : IBilleteraService
{
    private readonly BilleteraDbContext _db;

    public BilleteraService(BilleteraDbContext db)
    {
        _db = db;
    }

    public async Task<List<Cuenta>> ObtenerCuentas()
    {
        return await _db.Cuentas.Include(c => c.Movimientos).ToListAsync();
    }

    public async Task<Cuenta?> ObtenerCuenta(int id)
    {
        return await _db.Cuentas.Include(c => c.Movimientos).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Extracto?> GenerarExtracto(int id)
    {
        var cuenta = await _db.Cuentas.Include(c => c.Movimientos).FirstOrDefaultAsync(c => c.Id == id);
        if (cuenta == null)
            return null;

        return new Extracto
        {
            CuentaId = cuenta.Id,
            Titular = cuenta.NombreTitular,
            SaldoFinal = cuenta.Saldo,
            Historial = cuenta.Movimientos
        };
    }

    public async Task<string> CrearCuenta(CrearCuentaRequest req)
    {
        if (req.SaldoInicial < 0)
            return "Error: El saldo inicial no puede ser negativo.";

        var cuenta = new Cuenta
        {
            NombreTitular = req.NombreTitular,
            NumeroDocumento = req.NumeroDocumento,
            Saldo = req.SaldoInicial
        };

        _db.Cuentas.Add(cuenta);
        await _db.SaveChangesAsync();

        if (req.SaldoInicial > 0)
        {
            _db.Movimientos.Add(new Movimiento
            {
                CuentaId = cuenta.Id,
                Tipo = "Apertura",
                Monto = req.SaldoInicial,
                Descripcion = "Saldo inicial"
            });
            await _db.SaveChangesAsync();
        }

        return "OK";
    }

    public async Task<string> Depositar(int id, decimal monto, string desc)
    {
        if (monto <= 0)
            return "Error: El monto debe ser mayor a cero.";

        var cuenta = await _db.Cuentas.FindAsync(id);
        if (cuenta == null)
            return "Error: Cuenta no encontrada.";

        cuenta.Saldo += monto;

        _db.Movimientos.Add(new Movimiento
        {
            CuentaId = cuenta.Id,
            Tipo = "Deposito",
            Monto = monto,
            Descripcion = desc
        });

        await _db.SaveChangesAsync();
        return "OK";
    }

    public async Task<string> Retirar(int id, decimal monto, string desc)
    {
        const decimal comision = 1m;

        if (monto <= 0)
            return "Error: El monto debe ser mayor a cero.";

        var cuenta = await _db.Cuentas.FindAsync(id);
        if (cuenta == null)
            return "Error: Cuenta no encontrada.";

        if (cuenta.Saldo < monto + comision)
            return "Error: Fondos insuficientes.";
        cuenta.Saldo -= monto + comision;

        _db.Movimientos.Add(new Movimiento
        {
            CuentaId = cuenta.Id,
            Tipo = "Retiro",
            Monto = monto,
            Descripcion = desc
        });

        _db.Movimientos.Add(new Movimiento
        {
            CuentaId = cuenta.Id,
            Tipo = "Comision de retiro",
            Monto = comision,
            Descripcion = "Comision fija por retiro"
        });

        await _db.SaveChangesAsync();
        return "OK";
    }

    public async Task<string> Transferir(TransferenciaRequest req)
    {
        if (req.Monto <= 0)
            return "Error: El monto debe ser mayor a cero.";

        var origen = await _db.Cuentas.FindAsync(req.CuentaOrigenId);
        var destino = await _db.Cuentas.FindAsync(req.CuentaDestinoId);

        if (origen == null || destino == null)
            return "Error: Una de las cuentas no existe.";

        if (origen.Saldo < req.Monto)
            return "Error: Fondos insuficientes en la cuenta origen.";

        origen.Saldo -= req.Monto;
        destino.Saldo += req.Monto;

        _db.Movimientos.Add(new Movimiento
        {
            CuentaId = origen.Id,
            Tipo = "Transferencia Enviada",
            Monto = req.Monto,
            Descripcion = "Envio a " + destino.NombreTitular
        });

        _db.Movimientos.Add(new Movimiento
        {
            CuentaId = destino.Id,
            Tipo = "Transferencia Recibida",
            Monto = req.Monto,
            Descripcion = "Recibido de " + origen.NombreTitular
        });

        await _db.SaveChangesAsync();
        return "OK";
    }
}