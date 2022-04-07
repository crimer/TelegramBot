using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramBot.Helpers;
using TelegramBot.Models;
using TelegramBot.Repository;
using TelegramBot.Telegram.Commands.Common;
using VvsuParser;
using VvsuParser.Models;

namespace TelegramBot.Telegram.Commands;

/// <summary>
/// Команда телеграмм бота "Получения расписания Вгуэса"
/// </summary>
public class TgGetStudyScheduleCommand : PublicCommand
{
    private readonly RedisPersistenceRepository _redisPersistenceRepository;
    private readonly VvsuParserService _vvsuParserService;
    public override string Name => "/schedule";
        
    public TgGetStudyScheduleCommand(IServiceProvider serviceProvider, ITelegramBotClient botClient) : base(serviceProvider, botClient)
    {
        _redisPersistenceRepository = serviceProvider.GetRequiredService<RedisPersistenceRepository>();
        _vvsuParserService = serviceProvider.GetRequiredService<VvsuParserService>();
    }
        
    /// <inheritdoc />
    protected override async Task<Result> Execute(Chat chat, User senderUser, int replyMessageId, List<string> tgMessageEntities)
    {
        try
        {
            var currentGroupResult = await _redisPersistenceRepository.GetCurrentGroupAsync(chat.Id);
            if (currentGroupResult.IsFailed)
            {
                // реализация поиска группы и ее сохранение
            }

            var groupScheduleResult = await _redisPersistenceRepository.GetGroupScheduleAsync(currentGroupResult.Value);
            if (groupScheduleResult.IsFailed)
            {
                // лог
            }

            var scheduleMessage = CreateScheduleMessage(groupScheduleResult.Value);
            
            await this.BotClient.SendTextMessageAsync(chat.Id, scheduleMessage, replyToMessageId: replyMessageId);
                
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Команда '/roll' ошибка: {ex}");
        }
    }

    private string CreateScheduleMessage(List<VvsuStudyScheduleWeek> scheduleWeeks)
    {
        var message = new StringBuilder();
        
        
        
        return message.ToString();
    }
}