using DibBase.Infrastructure;
using DsCore.ApiClient;
using Microsoft.AspNetCore.Mvc;
using DibBase.Extensions;
using Microsoft.AspNetCore.Authorization;
using Payment = DsCore.Api.Models.Payment;
using Transaction = DsCore.Api.Models.Transaction;
using CyclicFee = DsCore.Api.Models.CyclicFee;
using DibBaseApi;
using DsCore.Events;

namespace DsCore.Api;

[ApiController]
[Route("[controller]")]
public class BillingController(
    Repository<Transaction> transactionRepo,
    Repository<CyclicFee> cyclicFeeRepo) : ControllerBase
{
    readonly Repository<Transaction> transactionRepo = transactionRepo;
    readonly Repository<CyclicFee> cyclicFeeRepo = cyclicFeeRepo;

    [Authorize]
    [HttpGet("money/{currencyGuid}")]
    public async Task<ActionResult<long>> GetMoney(Guid currencyGuid, CancellationToken ct)
    {
        var userGuid = HttpContext.GetUserGuid();
        if (userGuid == null) return Unauthorized();

        var transactions = await transactionRepo.GetAll(
            restrict: x => x.Payment.CurrencyId == currencyGuid.Deobfuscate().Id && x.Payment.UserId == ((Guid)userGuid).Deobfuscate().Id,
            expand: [x => x.Payment], ct: ct
        );

        return Ok(transactions.Sum(x => x.Payment.Value));
    }

    [Authorize]
    [HttpPost("pay")]
    public async Task<ActionResult<Guid?>> PayOnce(Payment payment, CancellationToken ct)
    {
        var userGuid = HttpContext.GetUserGuid();
        if (userGuid == null) return Unauthorized();

        payment.UserGuid = (Guid)userGuid;
        var transaction = new Transaction { Payment = payment };
        if (payment.Value < 0 && (await GetMoney(payment.CurrencyGuid, ct)).Value < payment.Value)
            return Ok(null);

        await transactionRepo.InsertAsync(transaction, ct);
        if (payment.Value > 0)
        {
            await transactionRepo.RegisterEvent(new ToppedUpEvent
            {
                CurrecyGuid = payment.CurrencyGuid,
                UserGuid = (Guid)userGuid,
                Value = payment.Value
            }, ct);
        }

        await transactionRepo.CommitAsync(ct);

        return Ok(transaction.Guid);
    }

    [Authorize]
    [HttpPost("cyclic-fee")]
    public async Task<ActionResult<Guid?>> AddCyclicFee(Payment payment, TimeSpan paymentInterval, CancellationToken ct)
    {
        var userGuid = HttpContext.GetUserGuid();
        if (userGuid == null) return Unauthorized();

        payment.UserGuid = (Guid)userGuid;
        if (payment.Value < 0 && (await GetMoney(payment.Currency.Guid, ct)).Value < payment.Value)
            return Ok(null);

        var cyclicFee = new CyclicFee { Payment = payment, PaymentInterval = paymentInterval};
        await cyclicFeeRepo.InsertAsync(cyclicFee, ct);
        await cyclicFeeRepo.CommitAsync(ct);

        return Ok(cyclicFee.Guid);
    }

    [Authorize]
    [HttpPost("cyclic-fee/{guid}/cancel")]
    public async Task<ActionResult> CancelCyclicFee(Guid guid, CancellationToken ct)
    {
        var userGuid = HttpContext.GetUserGuid();
        if (userGuid == null) return Unauthorized();

        var subscribction = await cyclicFeeRepo.GetById(guid.Deobfuscate().Id, [x => x.Payment], ct);
        if (subscribction?.Payment.UserGuid != userGuid) return Unauthorized();

        await cyclicFeeRepo.DeleteAsync(guid.Deobfuscate().Id, ct);
        return Ok();
    }

    [Authorize]
    [HttpGet("cyclic-fee/{guid}")]
    public async Task<ActionResult<CyclicFee>> GetCyclicFee(Guid guid, CancellationToken ct)
    {
        var userGuid = HttpContext.GetUserGuid();
        if (userGuid == null) return Unauthorized();

        var cyclicFee = await cyclicFeeRepo.GetById(guid.Deobfuscate().Id, [x => x.Payment], ct);
        if (cyclicFee == null) return NotFound();
        if (cyclicFee.Payment.UserGuid != userGuid) return Unauthorized();

        return Ok(IdHelper.HidePrivateId(cyclicFee));
    }

    [Authorize]
    [HttpGet("transaction/{guid}")]
    public async Task<ActionResult<Transaction>> GetTransaction(Guid guid, CancellationToken ct)
    {
        var userGuid = HttpContext.GetUserGuid();
        if (userGuid == null) return Unauthorized();

        var transaction = await transactionRepo.GetById(guid.Deobfuscate().Id, [x => x.Payment], ct);
        if (transaction == null) return NotFound();
        if (transaction.Payment.UserGuid != userGuid) return Unauthorized();

        return Ok(IdHelper.HidePrivateId(transaction));
    }
}