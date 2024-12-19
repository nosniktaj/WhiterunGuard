using Discord;

namespace WhiterunGuard
{
    internal class Program
    {
        private static readonly DiscordHandler _discord = new();
        private static TikTokHandler _tikTokHandler = null!;
        private static readonly ConsoleCommandHandler _commandHandler = new();

        public static void Main()
        {
            _discord.Client.Log += Log;
            _discord.Client.Ready += Client_Ready;
            while (_commandHandler.Running) ;
        }


        #region Custom Events

        private static void TikTokLiveStarted(object? sender, bool isLive) => _ = _discord.TikTokLiveStarted(isLive);

        #endregion

        #region Discord Net Events

        private static Task Client_Ready()
        {
            _tikTokHandler = new();
            _tikTokHandler.LiveStarted += TikTokLiveStarted;
            return Task.CompletedTask;
        }

        private static Task Log(LogMessage arg)
        {
            _commandHandler.WriteConsoleLine(arg.Message);
            return Task.CompletedTask;
        }

        #endregion
    }
}