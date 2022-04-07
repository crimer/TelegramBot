using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Telegram.Commands.Common;

/// <summary>
/// Класс публичной команды (для всех групповых чатов, каналов и личных переписок с ботом)
/// </summary>
public class PublicCommand : BaseCommand
{
    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="serviceProvider">Провайдер сервисв</param>
    /// <param name="botClient">Клиент телеграмм бота</param>
    /// <param name="tgMessageHelper">Класс помощник для сообщений</param>
    public PublicCommand(IServiceProvider serviceProvider, ITelegramBotClient botClient) : base(serviceProvider, botClient)
    {
    }
        
    /// <inheritdoc /> 
    protected override async Task<bool> CheckChatAccess(ChatType chatType, long chatId)
    {
        return true;
    }
}