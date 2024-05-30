using DibBase.Infrastructure;
using DsCore.Api.Models;

namespace DsCore.Api.Services;

class SubscriptionService(IServiceProvider sp) : BackgroundService
{
    readonly TimeSpan checkInterval = TimeSpan.FromMinutes(1);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            using var scope = sp.CreateScope();
            var cyclicFeeRepo = scope.ServiceProvider.GetRequiredService<Repository<CyclicFee>>();
            var transactionRepo = scope.ServiceProvider.GetRequiredService<Repository<Transaction>>();
            var unpaidCyclicFees = await cyclicFeeRepo.GetAll(restrict: x => x.UpdatedAt + x.PaymentInterval < DateTime.Now, expand: [x => x.Payment], ct: ct);
            foreach (var s in unpaidCyclicFees)
            {
                await cyclicFeeRepo.UpdateAsync(s, ct);
                await transactionRepo.InsertAsync(new() { PaymentId = s.PaymentId }, ct);
            }
            
            if (unpaidCyclicFees.Count != 0)
            {
                await cyclicFeeRepo.CommitAsync(ct); // :D/
                await transactionRepo.CommitAsync(ct);
            }
            
            await Task.Delay(checkInterval, ct);
        }
    }
}
