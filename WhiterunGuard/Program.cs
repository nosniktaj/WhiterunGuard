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
        private static ConsoleCommandHandler? _commandHandler;
        private static TikTokHandler? _tikTokHandler;

        private static DiscordHandler? _discord;

        public static void Main()
        {
            // Set up DI
            Client = new(new DiscordSocketConfig()
            {
                GatewayIntents = GatewayIntents.AllUnprivileged,
                LogGatewayIntentWarnings = true
            });
            _services = new ServiceCollection()
                .AddSingleton(Client)
                .AddSingleton<InteractionService>()
                .AddSingleton<ConfigManager>() // Register ConfigManager
                .AddSingleton<DiscordHandler>() // Register the main handler
                .AddSingleton<TikTokHandler>() // Register other dependencies
                .AddSingleton<ConsoleCommandHandler>()
                .BuildServiceProvider();
            Client.Log += Log;
            Client.Ready += Client_Ready;

            _commandHandler = _services.GetRequiredService<ConsoleCommandHandler>();
            _discord = _services.GetRequiredService<DiscordHandler>();
            _tikTokHandler = _services.GetRequiredService<TikTokHandler>();

            _tikTokHandler.LiveStarted += TikTokLiveStarted;
            while (_commandHandler.Running) ;

            Client?.Dispose();
        }

        private static Task Client_Ready() =>
            //_services.GetRequiredService<ConfigManager>();
            Task.CompletedTask;

        private static Task Log(LogMessage arg)
        {
            _commandHandler!.WriteConsoleLine(arg.Message);
            return Task.CompletedTask;
        }

        private static void TikTokLiveStarted(object? sender, bool isLive) => _ = _discord.TikTokLiveStarted(isLive);
    }
}