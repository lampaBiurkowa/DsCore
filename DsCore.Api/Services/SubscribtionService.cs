using DibBase.Infrastructure;
using DsCore.Api.Models;

namespace DsCore.Services;

class SubscribtionService(Repository<Subscribtion> subscribtionRepo, Repository<Transaction> transactionRepo) : BackgroundService
{
    readonly Repository<Subscribtion> subscribtionRepo = subscribtionRepo;
    readonly Repository<Transaction> transactionRepo = transactionRepo;
    readonly TimeSpan checkInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var unpaidSubscribtions = await subscribtionRepo.GetAll(restrict: x => x.UpdatedAt + x.PaymentInterval < DateTime.Now, expand: [x => x.Payment]);
            foreach (var s in unpaidSubscribtions)
            {
                await subscribtionRepo.UpdateAsync(s, ct);
                await transactionRepo.InsertAsync(new() { PaymentId = s.PaymentId }, ct);
            }
            
            if (unpaidSubscribtions.Count != 0)
                await transactionRepo.CommitAsync(ct);
            
            await Task.Delay(checkInterval, ct);
        }
    }
}
