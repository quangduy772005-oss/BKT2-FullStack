using PCM.Core.Entities;
using PCM.Core.Enums;
using PCM.Core.Interfaces;
using System.Linq;

namespace PCM.Application.Services
{
    public class WalletService
    {
        public WalletService() { }

        public Task<DepositRequest?> CreateDepositRequestAsync(int memberId, decimal amount, string bankName, string accountNumber, string accountHolder, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<DepositRequest?>(null);
        }

        public Task<bool> ApproveDepositAsync(int depositRequestId, string approvedBy, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<bool> RejectDepositAsync(int depositRequestId, string rejectionReason, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(false);
        }

        public Task<decimal> GetBalanceAsync(int memberId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(0m);
        }

        public Task<IEnumerable<WalletTransaction>> GetTransactionHistoryAsync(int memberId, int pageIndex = 0, int pageSize = 20, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Enumerable.Empty<WalletTransaction>());
        }

        public Task<IEnumerable<DepositRequest>> GetPendingDepositsAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Enumerable.Empty<DepositRequest>());
        }
    }
}
