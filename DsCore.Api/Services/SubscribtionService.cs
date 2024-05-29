using DibBase.Infrastructure;
using DsCore.Api.Models;

namespace DsCore.Services;

class SubscriptionService(Repository<Subscription> subscriptionRepo, Repository<Transaction> transactionRepo) : BackgroundService
{
    readonly Repository<Subscription> subscriptionRepo = subscriptionRepo;
    readonly Repository<Transaction> transactionRepo = transactionRepo;
    readonly TimeSpan checkInterval = TimeSpan.FromSeconds(5);

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            var unpaidSubscriptions = await subscriptionRepo.GetAll(restrict: x => x.UpdatedAt + x.PaymentInterval < DateTime.Now, expand: [x => x.Payment]);
            foreach (var s in unpaidSubscriptions)
            {
                await subscriptionRepo.UpdateAsync(s, ct);
                await transactionRepo.InsertAsync(new() { PaymentId = s.PaymentId }, ct);
            }
            
            if (unpaidSubscriptions.Count != 0)
                await transactionRepo.CommitAsync(ct);
            
            await Task.Delay(checkInterval, ct);
        }
    }
}
