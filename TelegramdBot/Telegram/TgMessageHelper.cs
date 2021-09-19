using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Telegram
{
    public class TgMessageHelper
    {
        private readonly TelegramBotClient _telegramBot;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="telegramBot">Клиент бота</param>
        public TgMessageHelper(TelegramBotClient telegramBot)
        {
            _telegramBot = telegramBot;
        }

        /// <summary>
        /// Отправка сообщения ботом с авто-удалением сообщения
        /// </summary>
        /// <param name="chatId">Идентификатор чата</param>
        /// <param name="text">Текст сообщения</param>
        /// <param name="replyMessageId">Идентификатор ответного сообщения</param>
        /// <param name="parseMode">Мод парсинга сообщений</param>
        public async Task SendMessageAsync(long chatId, string text, int replyMessageId, ParseMode parseMode = ParseMode.Html)
        {
            var sendedMessage = await _telegramBot.SendTextMessageAsync(chatId, text, parseMode: parseMode, replyToMessageId: replyMessageId);
            if(sendedMessage != null)
                DeleteMessageAsync(chatId, sendedMessage.MessageId);
        }

        /// <summary>
        /// Отправка сообщения ботом с авто-удалением сообщения
        /// </summary>
        /// <param name="chatId">Идентификатор чата</param>
        /// <param name="text">Текст сообщения</param>
        /// <param name="parseMode">Мод парсинга сообщений</param>
        public async Task SendMessageAsync(long chatId, string text, ParseMode parseMode = ParseMode.Html)
        {
            var sendedMessage = await _telegramBot.SendTextMessageAsync(chatId, text, parseMode: parseMode);
            if(sendedMessage != null)
                DeleteMessageAsync(chatId, sendedMessage.MessageId);
        }
        
        /// <summary>
        /// Авто-удаление сообщения по истечению минуты
        /// </summary>
        /// <param name="chatId">Идентификатор чата</param>
        /// <param name="messageId">Идентификатор сообщения</param>
        private Task DeleteMessageAsync(long chatId, int messageId)
        {
            return Task.Delay(TimeSpan.FromMinutes(1)).ContinueWith(async (_) =>
            {
                await _telegramBot.DeleteMessageAsync(chatId, messageId);
            });
        }
    }
}