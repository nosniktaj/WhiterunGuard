using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace WhiterunGuard
{
    internal class Program
    {
        private static DiscordSocketClient _client = null!;
        private static TikTokHandler _tikTokHandler = null!;

        private static IUser _adminUser = null!;

        private static readonly string _discordToken =
            Environment.GetEnvironmentVariable("WHITERUN_DISCORD_TOKEN") ?? string.Empty;

        private static readonly bool _running = true;

        public static async Task Main()
        {
            await startClient();

            while (_running)
            {
                var command = Console.ReadLine();
                switch (command.ToLowerInvariant())
                {
                    case "end":
                        Console.WriteLine("> Are you sure you want to end the program? (Y/N)");
                        break;
                }
                //Console Control Code
            }
        }

        #region Custom Events

        private static async void TikTokLiveStarted(object? sender, EventArgs e)
        {
#if DEBUG
        var x = await _client.GetUserAsync(617471240667398154).Result
            .SendMessageAsync("Lyla is now live on TikTok! \n https://www.tiktok.com/@lylaskyrim/live");
#else
            await _client.GetGuild(1205836076187394079).GetTextChannel(1205836076728451104)
                .SendMessageAsync("@everyone Lyla is now live on TikTok! \n https://www.tiktok.com/@lylaskyrim/live");
#endif
        }

        #endregion

        #region Private Static Methods

        private static async Task startClient()
        {
            if (string.IsNullOrWhiteSpace(_discordToken))
            {
                Console.WriteLine("No valid Discord Token has been provided.");
            }
            else
            {
                _client = new DiscordSocketClient();
                _client.Log += Log;
                _client.Ready += Client_Ready;
                _client.SlashCommandExecuted += SlashCommandHandler;

                await _client.LoginAsync(TokenType.Bot, _discordToken);
                await _client.StartAsync();
            }
        }

        #endregion

        #region Standard Events

        private static async Task Client_Ready()
        {
            _tikTokHandler = new();
            _tikTokHandler.LiveStarted += TikTokLiveStarted;

            var guild = _client.GetGuild(1205836076187394079);
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
        }

        private static Task Log(LogMessage arg)
        {
            Console.WriteLine($" {arg.Message}");
            _ = Console.ReadLine();
            Console.Write("> ");
            return Task.CompletedTask;
        }

        #endregion

        #region Commands

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