using DibBase.Infrastructure;
using DsCore.Api.Models;
using DsCore.ApiClient;
using Microsoft.AspNetCore.Mvc;
using DibBase.Extensions;
using Microsoft.AspNetCore.Authorization;
using Payment = DsCore.Api.Models.Payment;

namespace DsCore.Api;

[ApiController]
[Route("[controller]")]
public class BillingController(
    Repository<Transaction> transactionRepo,
    Repository<Subscription> subscriptionRepo) : ControllerBase
{
    readonly Repository<Transaction> transactionRepo = transactionRepo;
    readonly Repository<Subscription> subscriptionRepo = subscriptionRepo;

    [Authorize]
    [HttpGet("money/{currencyGuid}")]
    public async Task<ActionResult<long>> GetMoney(Guid currencyGuid, CancellationToken ct)
    {
        var userGuid = HttpContext.GetUserGuid();
        if (userGuid == null) return Unauthorized();

        var transactions = await transactionRepo.GetAll(restrict: x => x.Payment.CurrencyGuid == currencyGuid && x.Payment.UserGuid == userGuid, expand: [x => x.Payment], ct: ct);
        return Ok(transactions.Sum(x => x.Payment.Value));
    }

    [Authorize]
    [HttpPost("pay")]
    public async Task<ActionResult<long>> PayOnce(Payment payment, CancellationToken ct)
    {
        var userGuid = HttpContext.GetUserGuid();
        if (userGuid == null) return Unauthorized();

        var transaction = new Transaction { Payment = payment };
        await transactionRepo.InsertAsync(transaction, ct);
        await transactionRepo.CommitAsync(ct);

        return Ok();
    }

    [Authorize]
    [HttpPost("subscribe")]
    public async Task<ActionResult<long>> Subscribe(Payment payment, CancellationToken ct)
    {
        var userGuid = HttpContext.GetUserGuid();
        if (userGuid == null) return Unauthorized();

        var subscription = new Subscription { Payment = payment };
        await subscriptionRepo.InsertAsync(subscription, ct);
        await subscriptionRepo.CommitAsync(ct);

        return Ok();
    }

    [Authorize]
    [HttpPost("subscribction/{guid}/cancel")]
    public async Task<ActionResult<long>> Subscribe(Guid guid, CancellationToken ct)
    {
        var userGuid = HttpContext.GetUserGuid();
        if (userGuid == null) return Unauthorized();

        var subscribction = await subscriptionRepo.GetById(guid.Deobfuscate().Id, [x => x.Payment], ct);
        if (subscribction?.Payment.UserGuid != userGuid) return Unauthorized();

        await subscriptionRepo.DeleteAsync(guid.Deobfuscate().Id, ct);
        return Ok();
    }
}