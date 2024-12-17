using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace WhiterunGuard
{
    public class DiscordHandler
    {
        #region Public Properties

        public DiscordSocketClient Client = null!;

        #endregion

        #region Constructor

        public DiscordHandler() => _ = StartClient();

        #endregion

        #region Public Methods

        public async Task TikTokLiveStarted()
        {
#if DEBUG
            await Client.GetUserAsync(617471240667398154).Result
                .SendMessageAsync("Lyla is now live on TikTok! \n https://www.tiktok.com/@lylaskyrim/live");
#else
            await Client.GetGuild(1205836076187394079).GetTextChannel(1205836076728451104)
                .SendMessageAsync("@everyone Lyla is now live on TikTok! \n https://www.tiktok.com/@lylaskyrim/live");
#endif
        }

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
            var guild = Client.GetGuild(1205836076187394079);

            #region SayCommand

            var sayCommand = new SlashCommandBuilder();
            sayCommand.WithName("say");
            sayCommand.WithDescription("Say a message");
            sayCommand.AddOption("message", ApplicationCommandOptionType.String, "The message you want to say",
                true);
            sayCommand.WithDefaultMemberPermissions(GuildPermission.Administrator);
            try
            {
                await guild.CreateApplicationCommandAsync(sayCommand.Build());
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
            }
        }

        private static async Task Say(SocketSlashCommand command)
        {
            var message = (string)command.Data.Options.First().Value;
            var sender = (SocketGuildUser)command.User;
            if (sender.IsAdmin())
                await command.RespondAsync(message);
            else
                await command.RespondAsync("Sorry, you don't have permission to use this command.");
        }

        #endregion
    }
}