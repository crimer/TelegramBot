using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Telegram.Commands.Common
{
    /// <summary>
    /// Класс публичной команды (для всех групповых чатов, каналов и личных переписок с ботом)
    /// </summary>
    public class PublicCommand : BaseCommand
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="botClient">Клиент телеграмм бота</param>
        /// <param name="tgMessageHelper">Класс помощник для сообщений</param>
        public PublicCommand(TelegramBotClient botClient, TgMessageHelper tgMessageHelper) : base(botClient, tgMessageHelper)
        {
        }
        
        /// <inheritdoc /> 
        protected override async Task<bool> CheckChatAccess(ChatType chatType, long chatId)
        {
            return true;
        }
    }
}