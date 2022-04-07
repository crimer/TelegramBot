using System;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace TelegramBot.Telegram.Commands.Common;

/// <summary>
/// Фабрика создания классов обработчиков команд бота
/// </summary>
public class TgCommandFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ITelegramBotClient _telegramBotClient;

    public TgCommandFactory(IServiceProvider serviceProvider, ITelegramBotClient telegramBotClient)
    {
        _serviceProvider = serviceProvider;
        _telegramBotClient = telegramBotClient;
    }
        
    /// <summary>
    /// Создание классов обработчиков команд бота
    /// </summary>
    /// <param name="telegramMessage">Текст сообщения я телеграмм</param>
    public BaseCommand GetCommand(string telegramMessage)
    {
        using var _ = _serviceProvider.CreateScope();
        var scope = _serviceProvider.CreateScope();
        
        switch (telegramMessage)
        {
            // case "/roll":
            //     return new TgRollCommand(scope.ServiceProvider, _telegramBotClient);
            // case "/8ball":
            //     return new TgMagicBallCommand(scope.ServiceProvider, _telegramBotClient);
            // case "/decide":
            //     return new TgDecideCommand(scope.ServiceProvider, _telegramBotClient);
            case "/schedule":
                return new TgGetStudyScheduleCommand(scope.ServiceProvider, _telegramBotClient);
            default:
                return null;
        }
    }
}