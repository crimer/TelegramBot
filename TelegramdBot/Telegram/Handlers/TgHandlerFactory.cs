using Telegram.Bot.Types.Enums;

namespace TelegramBot.Telegram.Handlers;

public class TgHandlerFactory
{
    private readonly TextMessageTgHandler _textMessageTgHandler;
    private readonly NoneActionTgHandler _noneActionTgHandler;

    public TgHandlerFactory(
        TextMessageTgHandler textMessageTgHandler,
        NoneActionTgHandler noneActionTgHandler)
    {
        _textMessageTgHandler = textMessageTgHandler;
        _noneActionTgHandler = noneActionTgHandler;
    }
    
    public ITgHandler GetHandler(UpdateType updateType) =>
        updateType switch
        {
            UpdateType.Message => _textMessageTgHandler,
            _ => _noneActionTgHandler
        };
}