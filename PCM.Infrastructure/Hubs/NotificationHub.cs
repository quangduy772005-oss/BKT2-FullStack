using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace PCM.Infrastructure.Hubs;

public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation($"User {Context.User?.FindFirst("sub")?.Value} connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation($"User {Context.User?.FindFirst("sub")?.Value} disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinRoom(string roomName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("UserJoined", Context.User?.FindFirst("sub")?.Value);
    }

    public async Task LeaveRoom(string roomName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
        await Clients.Group(roomName).SendAsync("UserLeft", Context.User?.FindFirst("sub")?.Value);
    }

    // Notification methods
    public async Task SendNotification(string message)
    {
        await Clients.All.SendAsync("ReceiveNotification", message);
    }

    public async Task NotifyBookingUpdate(int bookingId, string status)
    {
        await Clients.All.SendAsync("BookingUpdated", bookingId, status);
    }

    public async Task NotifyMatchResult(int matchId, int team1Score, int team2Score, string result)
    {
        await Clients.All.SendAsync("MatchResultUpdated", matchId, team1Score, team2Score, result);
    }

    public async Task NotifyWalletUpdate(int memberId, decimal newBalance)
    {
        await Clients.User(memberId.ToString()).SendAsync("WalletUpdated", newBalance);
    }

    public async Task NotifyDepositApproved(int memberId, decimal amount)
    {
        await Clients.User(memberId.ToString()).SendAsync("DepositApproved", amount);
    }
}
