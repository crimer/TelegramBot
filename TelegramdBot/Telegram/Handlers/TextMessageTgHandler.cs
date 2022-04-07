using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Extensions;
using TelegramBot.Helpers;
using TelegramBot.Telegram.Commands.Common;

namespace TelegramBot.Telegram.Handlers;

public class TextMessageTgHandler : ITgHandler
{
    private readonly ILogger<TextMessageTgHandler> _logger;
    private readonly TgCommandFactory _tgCommandFactory;

    public TextMessageTgHandler(ILogger<TextMessageTgHandler> logger, TgCommandFactory tgCommandFactory)
    {
        _logger = logger;
        _tgCommandFactory = tgCommandFactory;
    }
    
    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        try
        {
            if(update.Message == null || update.Message.Text == null ||
               !IsBotCommand(update.Message) || update.Message.Chat.Type == ChatType.Private)
                return;

            var chat = update.Message.Chat;
            var userFrom = update.Message.From;
            
            _logger.LogInformation($"[{chat.Title} - {TelegramHelpers.MakeUserName(userFrom)}] {update.Message.Text}");

            var t = ParseMessage(update.Message);
            
            var commandText = update.Message.Text.Split("@").FirstOrDefault();
            var command = _tgCommandFactory.GetCommand(commandText);
            if (command == null)
            {
                _logger.LogError($"Подходящая команда не найдена");
                return;
            }

            var messageEntities = new List<string>();
            
            var commandResult = await command.ExecuteAsync(chat, userFrom, update.Message.MessageId, messageEntities);
            if (commandResult.IsFailed)
            {
                _logger.LogError(commandResult.ToString());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"При обработке текстового сообщения произошла ошибка: {ex}");
        }
    }

    private bool IsBotCommand(Message message)
    {
        if (message.Entities.IsNullOrEmpty())
            return false;

        return message.Entities != null && message.Entities.Any(e => e.Type == MessageEntityType.BotCommand);
    }
    
    /// <summary>
    /// Парсинг сообщения
    /// </summary>
    /// <param name="telegramMessage">Сообщение</param>
    /// <returns>Спосок элементов сообщения</returns>
    private Result<List<string>> ParseMessage(Message telegramMessage)
    {
        try
        {
            var messages = new List<string>();
            
            if (telegramMessage.Entities.IsNullOrEmpty()
                || telegramMessage.EntityValues.IsNullOrEmpty()
                || telegramMessage.Entities.Length != telegramMessage.EntityValues.Count())
                    
                return Result.Ok(messages);
    
            var text = telegramMessage.EntityValues.ToArray();
            var rawMessage = telegramMessage.Text;
                
            for (int i = 0; i < telegramMessage.Entities.Length; i++)
            {
                messages.Add(text[i]);
                rawMessage = rawMessage.Replace(text[i], "").TrimStart();
            }
                
            var messageArgs = rawMessage.Split(" ");
    
            messages.AddRange(messageArgs);
                
            return Result.Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Приозошла ошибка во время обработки поступления нового пользователя: {ex}");
            return Result.Fail("Приозошла ошибка во время обработки поступления нового пользователя");
        }
    }
}