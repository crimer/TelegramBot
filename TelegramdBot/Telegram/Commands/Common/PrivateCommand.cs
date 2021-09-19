using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Telegram.Commands.Common
{
    /// <summary>
    /// Класс приватной команды (только для личной переписки)
    /// </summary>
    public class PrivateCommand : BaseCommand
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="botClient">Клиент телеграмм бота</param>
        /// <param name="tgMessageHelper">Класс помощник для сообщений</param>
        public PrivateCommand(TelegramBotClient botClient, TgMessageHelper tgMessageHelper) : base(botClient, tgMessageHelper)
        {
        }
        
        /// <inheritdoc />
        protected override async Task<bool> CheckChatAccess(ChatType chatType, long chatId)
        {
            if (chatType != ChatType.Private)
            {
                await this.BotClient.SendTextMessageAsync(chatId, $"Данная команда доступна только в личной переписке с ботом");
                return false;
            }
            return true;
        }
    }
}