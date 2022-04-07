namespace TelegramBot.Models;

/// <summary>
/// Модель участника чата
/// </summary>
public class TgChatMember
{
    /// <summary>
    /// Идентификатор чата
    /// </summary>
    public long ChatId { get; set; }
        
    /// <summary>
    /// Идентификатор пользователя
    /// </summary>
    public long UserId { get; set; }
        
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public string Name { get; set; }
}