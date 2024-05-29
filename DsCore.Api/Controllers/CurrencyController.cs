using DibBase.Infrastructure;
using DsCore.Api.Models;
using Microsoft.AspNetCore.Mvc;
using DibBase.Extensions;
using DibBaseApi;

namespace DsCore.Api;

[ApiController]
[Route("[controller]")]
public class CurrencyController(Repository<Currency> currencyRepo) : ControllerBase
{
    readonly Repository<Currency> currencyRepo = currencyRepo;

    [HttpGet("currency")]
    public async Task<ActionResult<List<Currency>>> GetCurrencies(CancellationToken ct) => await currencyRepo.GetAll(ct: ct);

    [HttpGet("currency/{guid}")]
    public async Task<ActionResult<Currency?>> GetCurrency(Guid guid, CancellationToken ct)
    {
        var currency = await currencyRepo.GetById(guid.Deobfuscate().Id, ct: ct);
        if (currency == null) return Ok(null);

        return Ok(IdHelper.HidePrivateId(currency));
    }

    [HttpGet("currency/name/{name}")]
    public async Task<ActionResult<Currency?>> GetCurrency(string name, CancellationToken ct)
    {
        var currency = (await currencyRepo.GetAll(restrict: x => x.Name == name, ct: ct)).FirstOrDefault();
        if (currency == null) return Ok(null);

        return Ok(IdHelper.HidePrivateId(currency));
    }   
}