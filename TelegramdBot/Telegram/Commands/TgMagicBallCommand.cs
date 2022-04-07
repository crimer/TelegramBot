using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Extensions;
using TelegramBot.Helpers;
using TelegramBot.Models;
using TelegramBot.Telegram.Commands.Common;

namespace TelegramBot.Telegram.Commands;

/// <summary>
/// Команда телеграмм бота "Магический шар"
/// </summary>
public class TgMagicBallCommand : PublicCommand
{
    public override string Name => "/8ball";
    public TgMagicBallCommand(IServiceProvider serviceProvider, ITelegramBotClient botClient) : base(serviceProvider, botClient) { }
        
    /// <inheritdoc />
    protected override async Task<Result> Execute(Chat chat, User senderUser, int replyMessageId, List<string> tgMessageEntities)
    {
        try
        {
            if (tgMessageEntities.IsNullOrEmpty())
            {
                await this.BotClient.SendTextMessageAsync(chat.Id, $"Нет вашего текста для предсказания ", replyToMessageId: replyMessageId, parseMode: ParseMode.MarkdownV2);
                return Result.Ok();
            }
                
            var answers = new []
            {
                "Вперед!",
                "Не сейчас",
                "Не делай этого",
                "Ты шутишь?",
                "Да, но позднее",
                "Думаю, не стоит",
                "Не надейся на это",
                "Ни в коем случае",
                "Это неплохо",
                "Кто знает?",
                "Туманный ответ, попробуй еще",
                "Я не уверен",
                "Я думаю, хорошо",
                "Забудь об этом",
                "Это возможно",
                "Определенно - да",
                "Быть может",
                "Слишком рано",
                "Да",
                "Конечно, да",
                "Даже не думай",
                "Лучше Вам пока этого не знать",
                "Без понятия",
                "ХЗ"
            };
            
            var answer = answers.Random();
            await this.BotClient.SendTextMessageAsync(chat.Id, $"🔮 **{answer}** 🔮", replyToMessageId: replyMessageId, parseMode: ParseMode.MarkdownV2);

            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Команда '/8ball' ошибка: {ex}");
        }
    }
}