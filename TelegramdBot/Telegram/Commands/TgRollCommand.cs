using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentResults;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Models;
using TelegramBot.Telegram.Commands.Common;

namespace TelegramBot.Telegram.Commands
{
    /// <summary>
    /// Команда телеграмм бота "Бросить кости"
    /// </summary>
    public class TgRollCommand : PublicCommand
    {
        public override string Name => "/roll";
        public TgRollCommand(TelegramBotClient botClient, TgMessageHelper tgMessageHelper) : base(botClient, tgMessageHelper) { }
        
        /// <inheritdoc />
        protected override async Task<Result> Execute(Chat chat, User senderUser, ChatMember targetUser, int replyMessageId, List<TgMessageEntity> tgMessageEntities)
        {
            try
            {
                var maxRandomValue = 20;

                var maxValue = tgMessageEntities.FirstOrDefault(m => m.Type == MessageEntityType.Unknown);
                if (maxValue != null && !string.IsNullOrWhiteSpace(maxValue.Text))
                {
                    if (!int.TryParse(maxValue.Text, out maxRandomValue))
                    {
                        await TgMessageHelper.SendMessageAsync(chat.Id, $"Вторым аргументом команды должно быть целое число", replyMessageId);
                        return Result.Ok();
                    }
                }
                await TgMessageHelper.SendMessageAsync(chat.Id, $"🎲 Шанс {new Random().Next(0, maxRandomValue)} из {maxRandomValue}", replyMessageId);
                
                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Команда '/roll' ошибка: {ex}");
            }
        }
    }
}