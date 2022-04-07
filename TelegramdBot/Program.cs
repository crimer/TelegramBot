using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types.Enums;
using TelegramBot.Di;
using TelegramBot.Telegram;

public class Program
{
    public static async Task Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(serviceCollection =>
            {
                serviceCollection.ConfigureServices();
            })
            .Build();

        var telegramBotClient = host.Services.GetService<ITelegramBotClient>();
        var handler = host.Services.GetRequiredService<TelegramUpdateHandler>();
            
        telegramBotClient.StartReceiving(handler, new ReceiverOptions
        {
            AllowedUpdates = new []
            {
                UpdateType.Message,
                UpdateType.CallbackQuery,
                UpdateType.ChatMember,
                UpdateType.MyChatMember
            }
        });
            
        await host.RunAsync();
    }
}
