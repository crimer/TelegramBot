using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Helpers;

public static class TgMessageHelper
{
    
    /// <summary>
    /// Отправка сообщения ботом с авто-удалением сообщения
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="text">Текст сообщения</param>
    /// <param name="replyMessageId">Идентификатор ответного сообщения</param>
    /// <param name="parseMode">Мод парсинга сообщений</param>
    public static async Task SendMessageAsync(this TelegramBotClient telegramBot, long chatId, string text, int replyMessageId, ParseMode parseMode = ParseMode.Html)
    {
        var sendedMessage = await telegramBot.SendTextMessageAsync(chatId, text, parseMode: parseMode, replyToMessageId: replyMessageId);
        if(sendedMessage != null)
            DeleteMessageAsync(telegramBot, chatId, sendedMessage.MessageId);
    }

    /// <summary>
    /// Отправка сообщения ботом с авто-удалением сообщения
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="text">Текст сообщения</param>
    /// <param name="parseMode">Мод парсинга сообщений</param>
    public static async Task SendMessageAsync(this TelegramBotClient telegramBot, long chatId, string text, ParseMode parseMode = ParseMode.Html)
    {
        var sendedMessage = await telegramBot.SendTextMessageAsync(chatId, text, parseMode: parseMode);
        if(sendedMessage != null)
            DeleteMessageAsync(telegramBot, chatId, sendedMessage.MessageId);
    }
        
    /// <summary>
    /// Авто-удаление сообщения по истечению минуты
    /// </summary>
    /// <param name="chatId">Идентификатор чата</param>
    /// <param name="messageId">Идентификатор сообщения</param>
    private static Task DeleteMessageAsync(this TelegramBotClient telegramBot, long chatId, int messageId)
    {
        return Task.Delay(TimeSpan.FromMinutes(1)).ContinueWith(async (_) =>
        {
            await telegramBot.DeleteMessageAsync(chatId, messageId);
        });
    }
}