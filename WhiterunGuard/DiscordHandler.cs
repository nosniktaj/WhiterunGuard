using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.Rest;
using Discord.WebSocket;
using Newtonsoft.Json;
using WhiterunConfig;

namespace WhiterunGuard
{
    public class DiscordHandler
    {
        #region Events

        public EventHandler<NewReactionRole> OnNewReactionRole = null!;

        #endregion

        #region Private Fields

        private readonly DiscordSocketClient _client;

        private readonly ConfigManager _configManager;

        private readonly InteractionService _interactionService;
        private readonly IServiceProvider _services;

        private RestUserMessage? _liveMessage;

        #endregion

        #region Constructor

        public DiscordHandler(DiscordSocketClient client, ConfigManager configManager, IServiceProvider services)
        {
            _configManager = configManager;
            _client = client;
            _services = services;
            _interactionService = new(_client.Rest, new()
            {
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Info
            });

            _ = StartClient();
        }

        #endregion

        #region Public Methods

        public async Task TikTokLiveStarted(bool isLive)
        {
#if DEBUG
            if (isLive)
                await _client.GetUserAsync(617471240667398154).Result
                    .SendMessageAsync("Lyla is now live on TikTok! \n https://www.tiktok.com/@lylaskyrim/live");
            else
                await _client.GetUserAsync(617471240667398154).Result.SendMessageAsync("Lyla's live has ended");
#else
            if (isLive)
                _liveMessage = _client.GetGuild(1205836076187394079).GetTextChannel(1205836076728451104)
                    .SendMessageAsync(
                        "@everyone Lyla is now live on TikTok! \n https://www.tiktok.com/@lylaskyrim/live").Result;
            else if (_liveMessage != null) await _liveMessage.DeleteAsync();


#endif
        }

        #endregion

        #region Private Methods

        #region Discord NET Events

        private async Task StartClient()
        {
            var discordToken = Environment.GetEnvironmentVariable("WHITERUN_DISCORD_TOKEN");
            if (string.IsNullOrWhiteSpace(discordToken))
            {
                Console.WriteLine("No valid Discord Token has been provided.");
            }
            else
            {
                _client.Ready += Client_Ready;
                //_client.SlashCommandExecuted += SlashCommandHandler;
                _client.InteractionCreated += HandleInteraction;

                await _client.LoginAsync(TokenType.Bot, discordToken);
                await _client.StartAsync();

                await _interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _services);
            }
        }

        private async Task Client_Ready()
        {
            _configManager.Load();

            foreach (var guild in _client.Guilds) await _interactionService.RegisterCommandsToGuildAsync(guild.Id);
        }

        private async Task HandleInteraction(SocketInteraction interaction)
        {
            var context = new SocketInteractionContext(_client, interaction);
            await _interactionService.ExecuteCommandAsync(context, _services);
        }


        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "say":
                    await Say(command);
                    break;
            }
        }

        #endregion

        #region Say

        private async Task Say(SocketSlashCommand command)
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

        #endregion
    }
}