using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace TelegramBot.Telegram;

public interface ITgHandler
{
    Task HandleAsync(Update update, CancellationToken cancellationToken);
}