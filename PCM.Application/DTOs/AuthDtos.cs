namespace PCM.Application.DTOs;

#region Auth DTOs

public class LoginRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public UserDto User { get; set; } = null!;
}

public class RegisterRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
}

public class RefreshTokenRequest
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}

public class RefreshTokenResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}

#endregion

#region Member DTOs

public class UserDto
{
    public string Id { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string FullName { get; set; } = null!;
}

public class MemberDto
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime JoinDate { get; set; }
    public double RankELO { get; set; }
    public decimal WalletBalance { get; set; }
    public string? AvatarUrl { get; set; }
}

public class UpdateMemberProfileRequest
{
    public string FullName { get; set; } = null!;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? AvatarUrl { get; set; }
}

#endregion

#region Wallet DTOs

public class DepositRequestDto
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public decimal RequestAmount { get; set; }
    public string Status { get; set; } = null!;
    public string? ProofImageUrl { get; set; }
    public DateTime RequestDate { get; set; }
}

public class CreateDepositRequestRequest
{
    public decimal Amount { get; set; }
    public string? ProofImageUrl { get; set; }
}

public class WalletTransactionDto
{
    public int Id { get; set; }
    public int MemberId { get; set; }
    public decimal Amount { get; set; }
    public string Type { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class ApproveDepositRequest
{
    public int DepositRequestId { get; set; }
    public string? RejectionReason { get; set; }
    public bool IsApproved { get; set; }
}

#endregion
