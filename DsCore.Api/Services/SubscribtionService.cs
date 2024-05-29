using DibBase.Infrastructure;
using DsCore.Api.Models;

namespace DsCore.Services;

class CyclicFeeService(Repository<CyclicFee> cyclicFeeRepo, Repository<Transaction> transactionRepo) : BackgroundService
{
    readonly Repository<CyclicFee> cyclicFeeRepo = cyclicFeeRepo;
    readonly Repository<Transaction> transactionRepo = transactionRepo;
    readonly TimeSpan checkInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var unpaidCyclicFees = await cyclicFeeRepo.GetAll(restrict: x => x.UpdatedAt + x.PaymentInterval < DateTime.Now, expand: [x => x.Payment]);
            foreach (var s in unpaidCyclicFees)
            {
                await cyclicFeeRepo.UpdateAsync(s, ct);
                await transactionRepo.InsertAsync(new() { PaymentId = s.PaymentId }, ct);
            }
            
            if (unpaidCyclicFees.Count != 0)
                await transactionRepo.CommitAsync(ct);
            
            await Task.Delay(checkInterval, ct);
        }
    }
}
