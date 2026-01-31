using PCM.Core.Interfaces;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace PCM.Infrastructure.Services;

public class SignalRService : ISignalRService
{
    private readonly IHubContext<PCM.Infrastructure.Hubs.NotificationHub>? _hubContext;
    private readonly ILogger<SignalRService> _logger;

    public SignalRService(IHubContext<PCM.Infrastructure.Hubs.NotificationHub>? hubContext = null, ILogger<SignalRService>? logger = null)
    {
        _hubContext = hubContext;
        _logger = logger!;
    }

    public async Task SendToAllAsync(string method, params object?[] args)
    {
        try
        {
            if (_hubContext != null)
                await _hubContext.Clients.All.SendCoreAsync(method, args);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error sending message to all clients");
        }
    }

    public async Task SendToUserAsync(string userId, string method, params object?[] args)
    {
        try
        {
            if (_hubContext != null)
                await _hubContext.Clients.User(userId).SendCoreAsync(method, args);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error sending message to user {UserId}", userId);
        }
    }

    public async Task SendToGroupAsync(string groupName, string method, params object?[] args)
    {
        try
        {
            if (_hubContext != null)
                await _hubContext.Clients.Group(groupName).SendCoreAsync(method, args);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error sending message to group {GroupName}", groupName);
        }
    }

    public async Task AddToGroupAsync(string connectionId, string groupName)
    {
        try
        {
            if (_hubContext != null)
                await _hubContext.Groups.AddToGroupAsync(connectionId, groupName);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error adding connection to group");
        }
    }

    public async Task RemoveFromGroupAsync(string connectionId, string groupName)
    {
        try
        {
            if (_hubContext != null)
                await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupName);
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error removing connection from group");
        }
    }
}
