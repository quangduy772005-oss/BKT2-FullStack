using PCM.Core.Enums;

namespace PCM.Core.Entities;

public class TransactionCategory : BaseEntity
{
    public string Name { get; set; } = null!;
    public TransactionType Type { get; set; }
}

public class WalletTransaction : BaseEntity
{
    public int MemberId { get; set; }
    public int? CategoryId { get; set; }
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public string? ReferenceId { get; set; } // ID of Booking/Match/Tournament
    public string? Description { get; set; }
    public string? EncryptedSignature { get; set; } // SHA256 hash for integrity check

    // Navigation
    public virtual Member Member { get; set; } = null!;
    public virtual TransactionCategory? Category { get; set; }
}

public class DepositRequest : BaseEntity
{
    public int MemberId { get; set; }
    public decimal RequestAmount { get; set; }
    public DepositRequestStatus Status { get; set; } = DepositRequestStatus.Pending;
    public string? ProofImageUrl { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedDate { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime RequestDate { get; set; }

    // Navigation
    public virtual Member Member { get; set; } = null!;
}
