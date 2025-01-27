using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using WhiterunConfig;

namespace WhiterunGuard
{
    internal class Program
    {
        #region Public Properties

        public static DiscordSocketClient Client;

        #endregion

        private static ServiceProvider _services;
        private static ConsoleCommandHandler? _commandHandler = new();
        private static TikTokHandler? _tikTokHandler;

        private static DiscordHandler? _discord;

        public static void Main()
        {
            Client = new(new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.All,
                LogGatewayIntentWarnings = true
            });
            _services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton<InteractionService>()
                .AddSingleton<ConfigManager>() // Register ConfigManager
                .AddSingleton<DiscordHandler>() // Register the main handler
                .AddSingleton<TikTokHandler>() // Register other dependencies
                .BuildServiceProvider();
            Client.Log += Log;

            _discord = _services.GetRequiredService<DiscordHandler>();
            _tikTokHandler = _services.GetRequiredService<TikTokHandler>();
            _tikTokHandler.LiveStarted += TikTokLiveStarted;
            while (true) ;
        }

        private static Task Log(LogMessage arg)
        {
            //Console.WriteLine(arg.Message);
            _commandHandler!.WriteConsoleLine(arg.Message);
            return Task.CompletedTask;
        }

        private static void TikTokLiveStarted(object? sender, bool isLive) => _ = _discord.TikTokLiveStarted(isLive);
    }
}