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
        private static TikTokHandler _tiktokHandler = null!;
        private static bool _running = true;
        public static async Task Main()
        {

            await startClient();
            _tiktokHandler = new TikTokHandler();

            while (_running)
            {
                
            }
        }

        #region Private Static Methods

        private static async Task foo()
        {
            int i = 0;
            while (true)
            {
                Console.WriteLine("foo");

                if (i >= 1 && _tiktokHandler != null)
                {
                    _tiktokHandler.Dispose();
                    _tiktokHandler = null;
                }

                else if (i >= 1 && _tiktokHandler == null)
                {
                    Console.WriteLine("TikTokHandler is null, reinitializing...");
                }

                i++;
                await Task.Delay(5000);
            }
        }


        private static async Task startClient()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;

            var token = "MTMxNDMzOTMyNjY1MzYzMjUzMw.GAW8x_.Sy1lxRU8zd51Yoo1xlsuIt0jnU3aC_OdkdQitE";
            
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

        }

        #endregion
        
        #region Standard Events
        private static async Task Client_Ready()
        {
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
            var channel = (SocketTextChannel)command.Channel;
            var sender = (SocketGuildUser)command.User;
            if (sender is not null && (sender.GuildPermissions.Administrator || sender.Roles.Any(r => r.Name.Equals("Lyla's Boyfriend", StringComparison.OrdinalIgnoreCase))))
            {
                await channel.SendMessageAsync(message);
                await command.RespondAsync($"Message sent.");
            }
            else
            {
                await command.RespondAsync("Sorry, you don't have permission to use this command.");
            }
        }
        #endregion
    }
}