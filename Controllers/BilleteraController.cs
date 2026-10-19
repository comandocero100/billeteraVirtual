using Microsoft.AspNetCore.Mvc;
using ApibilleteraVirtual.Models;
using ApibilleteraVirtual.Services;

namespace ApibilleteraVirtual.Controllers;

[ApiController]
[Route("api/billetera")]
public class BilleteraController : ControllerBase
{
    private readonly IBilleteraService _service;

    public BilleteraController(IBilleteraService service)
    {
        _service = service;
    }

    [HttpGet("cuentas")]
    public async Task<IActionResult> ObtenerCuentas()
    {
        var cuentas = await _service.ObtenerCuentas();
        return Ok(cuentas);
    }

    [HttpGet("cuentas/{id}")]
    public async Task<IActionResult> ObtenerCuenta(int id)
    {
        var cuenta = await _service.ObtenerCuenta(id);
        if (cuenta == null)
            return NotFound("Cuenta no encontrada");

        return Ok(cuenta);
    }

    [HttpPost("cuentas")]
    public async Task<IActionResult> CrearCuenta(CrearCuentaRequest req)
    {
        var res = await _service.CrearCuenta(req);
        if (res != "OK")
            return BadRequest(res);

        return Ok("Cuenta creada correctamente");
    }

    [HttpPost("cuentas/{id}/depositar")]
    public async Task<IActionResult> Depositar(int id, OperacionMontoRequest req)
    {
        var res = await _service.Depositar(id, req.Monto, req.Descripcion);
        if (res != "OK")
            return BadRequest(res);

        return Ok("Deposito realizado");
    }

    [HttpPost("cuentas/{id}/retirar")]
    public async Task<IActionResult> Retirar(int id, OperacionMontoRequest req)
    {
        var res = await _service.Retirar(id, req.Monto, req.Descripcion);
        if (res != "OK")
            return BadRequest(res);

        return Ok("Retiro realizado");
    }

    [HttpPost("transferir")]
    public async Task<IActionResult> Transferir(TransferenciaRequest req)
    {
        var res = await _service.Transferir(req);
        if (res != "OK")
            return BadRequest(res);

        return Ok("Transferencia realizada");
    }
}