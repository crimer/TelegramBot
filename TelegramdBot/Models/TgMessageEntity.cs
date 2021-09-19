using Telegram.Bot.Types.Enums;

namespace TelegramBot.Models
{
    /// <summary>
    /// Модель части сообщения
    /// </summary>
    public class TgMessageEntity
    {
        /// <summary>
        /// Текст части сообщения
        /// </summary>
        public string Text { get; set; }
        
        /// <summary>
        /// Тип части сообщения
        /// </summary>
        /// <remarks>
        /// В идеале свой нормальный enum определить
        /// </remarks>
        public MessageEntityType Type { get; set; }
    }
}