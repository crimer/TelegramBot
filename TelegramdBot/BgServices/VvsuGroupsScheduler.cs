using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using TelegramBot.Helpers;

namespace TelegramBot.BgServices;

public class VvsuGroupsScheduler : IHostedService
{
    private SynchronizedTimer _updateGroupseTimer;

    public VvsuGroupsScheduler()
    {
        
    }
    
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _updateGroupseTimer = new SynchronizedTimer(
            async (_) => await UpdateAllVvsuGroupsAsync(), null, TimeSpan.Zero, TimeSpan.FromHours(6));
        
        return Task.CompletedTask;
    }

    private async Task UpdateAllVvsuGroupsAsync()
    {
        
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _updateGroupseTimer?.Dispose();
        return Task.CompletedTask;
    }
}