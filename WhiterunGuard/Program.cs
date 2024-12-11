using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace WhiterunGuard
{
    internal class Program
    {
        private static DiscordSocketClient _client = null!;
        public static async Task Main()
        {
            _client = new DiscordSocketClient();
            _client.Log += Log;
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;

            var token = "MTMxNDMzOTMyNjY1MzYzMjUzMw.GAW8x_.Sy1lxRU8zd51Yoo1xlsuIt0jnU3aC_OdkdQitE";
            
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            
            
            await Task.Delay(-1);

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

        private static async Task Client_Ready()
        {
            var guild = _client.GetGuild(1205836076187394079);
            var guildCommand = new SlashCommandBuilder();
            guildCommand.WithName("say");
            guildCommand.WithDescription("Say a message");
            guildCommand.AddOption("message", ApplicationCommandOptionType.String, "The message you want to say",
                isRequired: true);
            await guild.DeleteApplicationCommandsAsync();
            try
            {
                await guild.CreateApplicationCommandAsync(guildCommand.Build());
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
    }
}