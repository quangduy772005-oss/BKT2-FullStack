namespace PCM.Core.Enums;

public enum TransactionType
{
    Deposit = 0,
    PayBooking = 1,
    ReceivePrize = 2,
    Refund = 3,
    AdminAdjustment = 4
}

public enum TransactionStatus
{
    Pending = 0,
    Success = 1,
    Failed = 2,
    Cancelled = 3
}
