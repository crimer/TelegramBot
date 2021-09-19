using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentResults;
using Microsoft.Extensions.Options;
using Telegram.Bot.Types;
using TelegramBot.Configuration;

namespace TelegramBot.Telegram
{
    /// <summary>
    /// Менеджер для взаимодействия с API телеграмма
    /// </summary>
    public class TelegramApiManager
    {
        private readonly string _phoneNumberForAuth;
        private readonly string _telegramApiHash;
        private readonly string _telegramApiId;

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="options">Конфигурация</param>
        public TelegramApiManager(IOptions<AppConfig> options)
        {
            _phoneNumberForAuth = options.Value.PhoneNumberForAuth;
            _telegramApiHash = options.Value.TelegramApiHash;
            _telegramApiId = options.Value.TelegramApiId;
        }

        /// <summary>
        /// Отправка кода авторизации на номер указанный в переменных окружения
        /// </summary>
        /// <returns>Хэш для авторизации</returns>
        public async Task<Result<string>> SendAuthCodeAsync()
        {
            using var client = GetTelegramClient();
            try
            {
                await client.ConnectAsync();
                return Result.Ok(await client.SendCodeRequestAsync(_phoneNumberForAuth));
            }
            catch (Exception e)
            {
                return Result.Fail<string>($"При выполнении запроса произошла ошибка: {e}");
            }
        }

        /// <summary>
        /// Авторизация в Telegram API
        /// При успешной авторизации будет создан файл session.dat, который необходим для совершения запросов
        /// для получения списка пользователей в группе, либо для получения AccessHash для получения списка пользователей
        /// в супергруппах
        /// </summary>
        /// <param name="hash">Хэш авторизации</param>
        /// <param name="code">Код авторизации, который был отправлен при выполнении <see cref="SendAuthCodeAsync"/></param>
        public async Task<Result> AuthorizeAsync(string hash, string code)
        {
            using var client = GetTelegramClient();
            try
            {
                await client.ConnectAsync();
                await client.MakeAuthAsync(_phoneNumberForAuth, hash, code);
                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Fail<string>($"При выполнении запроса произошла ошибка: {e}");
            }
        }

        /// <summary>
        /// Получение и сохранение пользователей чата в хранилище.
        /// </summary>
        /// <param name="chatId">Идентификатор чата</param>
        /// <param name="accessHash">Хэш для доступа в группу (необходим только в случае получения пользователей супергруппы)</param>
        public async Task<Result> GetChatUsersAsync(long chatId, long accessHash)
        {
            using var client = GetTelegramClient();
            try
            {
                if (!client.IsUserAuthorized())
                    return Result.Fail("Для выполнения этой операции необходимо авторизоваться.");

                var isSuperGroup = chatId > int.MaxValue || chatId < int.MinValue;
                var formattedChatId = Math.Abs(isSuperGroup
                    ? int.Parse(chatId.ToString().Remove(0, 3))
                    : chatId);

                await client.ConnectAsync();

                List<ChatMember> users;
                if (isSuperGroup)
                {
                    var channel = new TLInputChannel
                    {
                        ChannelId = (int) formattedChatId,
                        AccessHash = accessHash
                    };

                    var requestGetParticipants = new TLRequestGetParticipants
                    {
                        Channel = channel,
                        Filter = new TLChannelParticipantsRecent(),
                        Limit = 300,
                        Offset = 0
                    };

                    var requestChannelFull = new TLRequestGetFullChannel
                    {
                        Channel = channel
                    };

                    var channelInfo = await client.SendRequestAsync<TLChatFull>(requestChannelFull);
                    var channelParticipants =
                        await client.SendRequestAsync<TLChannelParticipants>(requestGetParticipants);
                    var chatName = (channelInfo.Chats[0] as TLChat)?.Title ?? formattedChatId.ToString();

                    users = channelParticipants.Users
                        .Select(u => (TLUser) u)
                        .Where(u => !u.Bot)
                        .Select(u => new ChatMember(u.Id, User.GetUserName(u.Username, u.FirstName, u.LastName),
                            UserState.Code, new Chat(chatId, chatName, true)))
                        .ToList();
                }
                else
                {
                    var request = new TLRequestGetFullChat()
                    {
                        ChatId = (int) formattedChatId
                    };

                    var result = await client.SendRequestAsync<TLChatFull>(request);
                    var chatName = (result.Chats[0] as TLChat)?.Title ?? formattedChatId.ToString();

                    users = result.Users
                        .Select(u => (TLUser) u)
                        .Where(u => !u.Bot)
                        .Select(u => new ChatMember(u.Id, User.GetUserName(u.Username, u.FirstName, u.LastName),
                            UserState.Code, new Chat(chatId, chatName, false)))
                        .ToList();
                }

                await _usersRepository.CreateChatMemberIfNotExistAsync(users);

                return Result.Ok();
            }
            catch (Exception e)
            {
                return Result.Fail($"При выполнении запроса произошла ошибка: {e}");
            }
        }

        /// <summary>
        /// Получение хэш доступа для получения списка пользователей супергруппы
        /// </summary>
        /// <param name="superGroupId">Идентификатор супергруппы</param>
        public async Task<Result<string>> GetAccessHashForSuperGroupAsync(long superGroupId)
        {
            using var client = GetTelegramClient();
            try
            {
                if (!client.IsUserAuthorized())
                    return Result.Fail<string>("Для выполнения этой операции необходимо авторизоваться.");

                var isSuperGroup = superGroupId > int.MaxValue || superGroupId < int.MinValue;
                var formattedChatId = isSuperGroup
                    ? int.Parse(superGroupId.ToString().Remove(0, 3))
                    : superGroupId;


                await client.ConnectAsync();

                var superGroupInfo = (await client.SendRequestAsync<TLChats>(new TLRequestGetChannels
                {
                    Id = new TLVector<TLAbsInputChannel>(new List<TLAbsInputChannel>
                    {
                        new TLInputChannel
                        {
                            ChannelId = (int) formattedChatId
                        }
                    })
                }))?.Chats?.FirstOrDefault() as TLChannel;

                if (superGroupInfo?.AccessHash == null)
                    return Result.Fail<string>("Не удалось получить хэш доступа для группы");

                return Result.Ok(superGroupInfo.AccessHash.Value.ToString());
            }
            catch (Exception e)
            {
                return Result.Fail<string>($"При выполнении запроса произошла ошибка: {e}");
            }
        }

        private TelegramClient GetTelegramClient()
        {
            return new TelegramClient(int.Parse(_telegramApiId), _telegramApiHash);
        }
    }
}