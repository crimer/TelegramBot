using Telegram.Bot;

namespace TelegramBot.Telegram.Commands.Common
{
    /// <summary>
    /// Фабрика создания классов обработчиков команд бота
    /// </summary>
    public class TgCommandFactory
    {
        /// <summary>
        /// Создание классов обработчиков команд бота
        /// </summary>
        /// <param name="telegramMessage">Текст сообщения я телеграмм</param>
        /// <param name="botClient">Клиент бота</param>
        /// <param name="tgMessageHelper"></param>
        public static BaseCommand CreateCommand(string telegramMessage, TelegramBotClient botClient,
            TgMessageHelper tgMessageHelper)
        {
            switch (telegramMessage)
            {
                case "/roll":
                    return new TgRollCommand(botClient, tgMessageHelper);
                case "/8ball":
                    return new TgMagicBallCommand(botClient, tgMessageHelper);
                case "/decide":
                    return new TgDecideCommand(botClient, tgMessageHelper);
                default:
                    return null;
            }
        }
    }
}