using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentResults;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Telegram.Commands.Common;

/// <summary>
/// Базовая команда от телеграмм бота
/// </summary>
public abstract class BaseCommand
{
    /// <summary>
    /// Команда
    /// </summary>
    /// <example>/command</example>
    public virtual string Name { get; }

    protected IServiceProvider Services;
    protected ITelegramBotClient BotClient;

    /// <summary>
    /// Конструктор
    /// </summary>
    /// <param name="serviceProvider">Провайдер сервисов</param>
    /// <param name="botClient">Клиент телеграмм бота</param>
    /// <param name="tgMessageHelper">Класс помощник для сообщений</param>
    public BaseCommand(IServiceProvider serviceProvider, ITelegramBotClient botClient)
    {
        Services = serviceProvider;
        BotClient = botClient;
    }

    /// <summary>
    /// Метод обертка над вызовом обработчика команд
    /// </summary>
    /// <param name="chat">Чат</param>
    /// <param name="senderUser">Отправитель сообщения</param>
    /// <param name="targetUser">Упомянутый участник чата</param>
    /// <param name="replyMessageId">Идентификатор сообщения</param>
    /// <param name="tgMessageEntities">Список элементов сообщения</param>
    public async Task<Result> ExecuteAsync(Chat chat, User senderUser, int replyMessageId, List<string> tgMessageEntities)
    {
        var access = await CheckChatAccess(chat.Type, chat.Id);
        if (access)
            return await Execute(chat, senderUser, replyMessageId, tgMessageEntities);
        else
            return Result.Ok();
    }

    /// <summary>
    /// Метод обработки команд бота
    /// </summary>
    /// <param name="chat">Чат</param>
    /// <param name="senderUser">Отправитель команды</param>
    /// <param name="targetUser">На кого нацелена команда</param>
    /// <param name="replyMessageId">Идентификатор сообщения для ответа</param>
    /// <param name="tgMessageEntities">Список элементов сообщения</param>
    protected virtual async Task<Result> Execute(Chat chat, User senderUser, int replyMessageId, List<string> tgMessageEntities)
    {
        return Result.Ok();
    }

    /// <summary>
    /// Проверка доступа команды типу чата
    /// </summary>
    /// <param name="chatType">Тип чата</param>
    /// <param name="chatId">Идентификатор чата</param>
    protected virtual async Task<bool> CheckChatAccess(ChatType chatType, long chatId)
    {
        return true;
    }
}