using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PCM.Application.Services;

namespace PCM.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WalletController : ControllerBase
    {
        private readonly WalletService _walletService;

        public WalletController(WalletService walletService)
        {
            _walletService = walletService;
        }

        /// <summary>
        /// Request a deposit
        /// </summary>
        [HttpPost("deposit-request")]
        public async Task<IActionResult> CreateDepositRequest([FromBody] CreateDepositRequestDto request)
        {
            try
            {
                var memberId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                if (memberId == 0)
                    return Unauthorized();

                var depositRequest = await _walletService.CreateDepositRequestAsync(
                    memberId,
                    request.Amount,
                    request.BankName ?? "",
                    request.AccountNumber ?? "",
                    request.AccountHolder ?? ""
                );

                if (depositRequest == null)
                    return BadRequest("Không thể tạo yêu cầu nạp tiền");

                return Ok(new { id = depositRequest.Id, message = "Yêu cầu nạp tiền đã được tạo" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Approve a deposit request (admin only)
        /// </summary>
        [HttpPost("approve-deposit/{depositRequestId}")]
        [Authorize(Roles = "Admin,Treasurer")]
        public async Task<IActionResult> ApproveDeposit(int depositRequestId)
        {
            try
            {
                var approvedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _walletService.ApproveDepositAsync(depositRequestId, approvedBy);
                return Ok("Phê duyệt nạp tiền thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Reject a deposit request (admin only)
        /// </summary>
        [HttpPost("reject-deposit/{depositRequestId}")]
        [Authorize(Roles = "Admin,Treasurer")]
        public async Task<IActionResult> RejectDeposit(int depositRequestId, [FromBody] RejectDepositDto request)
        {
            try
            {
                await _walletService.RejectDepositAsync(depositRequestId, request.Reason ?? "");
                return Ok("Từ chối nạp tiền thành công");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get wallet balance
        /// </summary>
        [HttpGet("balance")]
        public async Task<IActionResult> GetBalance()
        {
            try
            {
                var memberId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
                if (memberId == 0)
                    return Unauthorized();

                var balance = await _walletService.GetBalanceAsync(memberId);
                return Ok(new { balance });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get transaction history
        /// </summary>
        [HttpGet("transactions")]
        public async Task<IActionResult> GetTransactions([FromQuery] int pageIndex = 0, [FromQuery] int pageSize = 20)
        {
            try
            {
                var userIdClaim = User.FindFirst("UserId")?.Value;
                var memberId = !string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var id) ? id : 0;
                
                if (memberId == 0)
                    return Unauthorized();

                var transactions = await _walletService.GetTransactionHistoryAsync(memberId, pageIndex, pageSize);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get pending deposits (admin only)
        /// </summary>
        [HttpGet("pending-deposits")]
        [Authorize(Roles = "Admin,Treasurer")]
        public async Task<IActionResult> GetPendingDeposits()
        {
            try
            {
                var deposits = await _walletService.GetPendingDepositsAsync();
                return Ok(deposits);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class CreateDepositRequestDto
    {
        public decimal Amount { get; set; }
        public string? BankName { get; set; }
        public string? AccountNumber { get; set; }
        public string? AccountHolder { get; set; }
    }

    public class RejectDepositDto
    {
        public string? Reason { get; set; }
    }
}
