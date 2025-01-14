using Discord;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace WhiterunGuard
{
    public class DiscordHandler
    {
        #region Events

        public static readonly EventHandler<NewReactionRole> OnNewReactionRole = null!;

        #endregion

        #region Public Properties

        public DiscordSocketClient Client = null!;

        #endregion

        #region Constructor

        public DiscordHandler() => _ = StartClient();

        #endregion

        #region Public Methods

        public async Task TikTokLiveStarted(bool isLive)
        {
#if DEBUG
            if (isLive)
                await Client.GetUserAsync(617471240667398154).Result
                    .SendMessageAsync("Lyla is now live on TikTok! \n https://www.tiktok.com/@lylaskyrim/live");
            else
                await Client.GetUserAsync(617471240667398154).Result.SendMessageAsync("Lyla's live has ended");
#else
            if (isLive)
                _liveMessage = Client.GetGuild(1205836076187394079).GetTextChannel(1205836076728451104)
                    .SendMessageAsync(
                        "@everyone Lyla is now live on TikTok! \n https://www.tiktok.com/@lylaskyrim/live").Result;
            else if (_liveMessage != null) await _liveMessage.Channel.DeleteMessageAsync(_liveMessage);


#endif
        }

        #endregion

        #region Private Properties

        private RestUserMessage? _liveMessage;

        private static SocketGuild? _guild;

        #endregion

        #region Private Methods

        private async Task StartClient()
        {
            var discordToken = Environment.GetEnvironmentVariable("WHITERUN_DISCORD_TOKEN");
            if (string.IsNullOrWhiteSpace(discordToken))
            {
                Console.WriteLine("No valid Discord Token has been provided.");
            }
            else
            {
                Client = new DiscordSocketClient();
                Client.Ready += Client_Ready;
                Client.SlashCommandExecuted += SlashCommandHandler;

                await Client.LoginAsync(TokenType.Bot, discordToken);
                await Client.StartAsync();
            }
        }

        private async Task Client_Ready()
        {
            _guild = Client.GetGuild(1205836076187394079);

            #region SayCommand

            var sayCommand = new SlashCommandBuilder();
            sayCommand.WithName("say");
            sayCommand.WithDescription("Say a message");
            sayCommand.AddOption("message", ApplicationCommandOptionType.String, "The message you want to say",
                true);
            try
            {
                await _guild.CreateApplicationCommandAsync(sayCommand.Build());
            }
            catch (HttpException ex)
            {
                var json = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }

            #endregion

            #region AddReactionRole

            var addReactionRoleCommand = new SlashCommandBuilder();
            addReactionRoleCommand.WithName("addreactionrole");
            addReactionRoleCommand.WithDescription("Add a reaction role");
            addReactionRoleCommand.AddOption("message-id", ApplicationCommandOptionType.String,
                "The ID of the message to process", true);
            addReactionRoleCommand.AddOption("role", ApplicationCommandOptionType.Role, "The role you wish to grant",
                true);
            addReactionRoleCommand.AddOption("emoji", ApplicationCommandOptionType.String, "The emoji for the reaction",
                true);
            try
            {
                await _guild.CreateApplicationCommandAsync(addReactionRoleCommand.Build());
            }
            catch (HttpException ex)
            {
                var json = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }

            #endregion
        }

        private static async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "say":
                    await Say(command);
                    break;
                case "addreactionrole":
                    await AddReactionRole(command);
                    break;
            }
        }

        private static async Task AddReactionRole(SocketSlashCommand command)
        {
            var messageIdOption = (string)command.Data.Options.FirstOrDefault(x => x.Name == "message-id")!.Value;
            var roleOption = (IRole)command.Data.Options.FirstOrDefault(x => x.Name == "role")!.Value;
            var emojiOption = (string)command.Data.Options.FirstOrDefault(x => x.Name == "emoji")!.Value;
            IMessage message = null!;
            if (ulong.TryParse(messageIdOption, out var messageId))
                message = command.Channel.GetMessageAsync(messageId).Result;

            if (message is not null)
                if (roleOption is not null)
                {
                    var emote = emojiOption.GetEmote();
                    if (emote is not null)
                    {
                        await command.RespondAsync($"Message: {message.Id}\n {roleOption.Mention}\n {emote}",
                            allowedMentions: AllowedMentions.None, ephemeral: true);
                        OnNewReactionRole?.Invoke(null, new NewReactionRole(emote, message, roleOption));
                    }

                    else
                    {
                        await command.RespondAsync("No Emote Detected", ephemeral: true);
                    }
                }
                else
                {
                    await command.RespondAsync("No Role Detected", ephemeral: true);
                }
            else
                await command.RespondAsync("No Message Detected", ephemeral: true);
        }

        private static async Task Say(SocketSlashCommand command)
        {
            var message = (string)command.Data.Options.First().Value;
            var sender = (SocketGuildUser)command.User;
            if (sender.IsAdmin())
            {
                await command.Channel.SendMessageAsync(message, allowedMentions: AllowedMentions.All);
                await command.RespondAsync(message, allowedMentions: AllowedMentions.None, ephemeral: true);
            }
            else
            {
                await command.RespondAsync("Sorry, you don't have permission to use this command.", ephemeral: true);
            }
        }

        #endregion
    }
}