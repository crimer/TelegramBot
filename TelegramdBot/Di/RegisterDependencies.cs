using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using TelegramBot.Configuration;
using TelegramBot.Repository;
using TelegramBot.Telegram;
using TelegramBot.Telegram.Commands;
using TelegramBot.Telegram.Commands.Common;
using TelegramBot.Telegram.Handlers;
using VvsuParser;
using VvsuParser.Selenium;
using VvsuParser.VvsuStudySchedule;

namespace TelegramBot.Di;

/// <summary>
/// Регистрация зависимостей приложения
/// </summary>
public static class RegisterDependencies
{
    private const string AppSettingsFileName = "appsettings.json";
        
    /// <summary>
    /// Конфигурация зависимостей приложения
    /// </summary>
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddDebug();
        });

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(AppSettingsFileName, optional: false)
            .Build();

        services.Configure<AppConfig>(configuration.GetSection("Config"));

        // Сервисы
        services.AddSingleton(sp => new SeleniumLoader(sp.GetRequiredService<IOptions<AppConfig>>().Value.VvsuScheduleTableUrl));
        services.AddSingleton<ITelegramBotClient>(sp => new TelegramBotClient(sp.GetRequiredService<IOptions<AppConfig>>().Value.BotToken));
        services.AddSingleton<TgHandlerFactory>();
        services.AddSingleton<TextMessageTgHandler>();
        services.AddSingleton<NoneActionTgHandler>();
        services.AddSingleton<TelegramUpdateHandler>();
        services.AddSingleton<VvsuStudyScheduleParser>();
        services.AddSingleton<VvsuParserService>();
        services.AddSingleton<RedisClient>();
        services.AddSingleton<RedisPersistenceRepository>();
        services.AddSingleton<TgCommandFactory>();
        services.AddSingleton<TgDecideCommand>();
        services.AddSingleton<TgRollCommand>();
        services.AddSingleton<TgMagicBallCommand>();
        services.AddSingleton<TgGetStudyScheduleCommand>();
    }
}