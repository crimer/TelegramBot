using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Telegram.Handlers;

public class NoneActionTgHandler : ITgHandler
{
    public Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}