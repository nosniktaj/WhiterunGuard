using System.Diagnostics;
using System.Runtime.CompilerServices;
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
        private static string _discordToken = Environment.GetEnvironmentVariable("DISCORD_TOKEN") ?? string.Empty;

        private static bool _running = true;
        
        public static async Task Main()
        {

            await startClient();

            while (_running)
            {
                //Console Control Code
            }
        }

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
            _tikTokHandler = new ();
            _tikTokHandler.LiveStarted += TikTokLiveStarted;
            
            var guild = _client.GetGuild(1205836076187394079);
            var sayCommand = new SlashCommandBuilder();
            sayCommand.WithName("say");
            sayCommand.WithDescription("Say a message");
            sayCommand.AddOption("message", ApplicationCommandOptionType.String, "The message you want to say",
                isRequired: true);
            await guild.DeleteApplicationCommandsAsync();
            try
            {
                await guild.CreateApplicationCommandAsync(sayCommand.Build());
            }
            catch(HttpException ex)
            {
                var json = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        private static Task Log(LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }
        #endregion 
        
        #region Custom Events
        private static async void TikTokLiveStarted(object? sender, EventArgs e)
        {
            #if DEBUG
                await _client.GetUserAsync(617471240667398154).Result.SendMessageAsync("Lyla is now live on TikTok! \n https://www.tiktok.com/@lylaskyrim/live");
            #else
                await  _client.GetGuild(1205836076187394079).GetTextChannel(1205836076728451104).SendMessageAsync("@everyone Lyla is now live on TikTok! \n https://www.tiktok.com/@lylaskyrim/live");
            #endif
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
            if (sender is not null && (sender.GuildPermissions.Administrator || sender.Roles.Any(r => r.Name.Equals("Lyla's Boyfriend", StringComparison.OrdinalIgnoreCase))))
            {
                await command.RespondAsync(message);
            }
            else
            {
                await command.RespondAsync("Sorry, you don't have permission to use this command.");
            }
        }
        #endregion
    }
}