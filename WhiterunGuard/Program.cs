using Discord;
using Discord.WebSocket;
using WhiterunConfig;

namespace WhiterunGuard
{
    internal class Program
    {
        #region Public Properties

        public static readonly DiscordSocketClient Client = new();

        #endregion


        private static DiscordHandler? _discord;
        private static TikTokHandler? _tikTokHandler;
        private static ConsoleCommandHandler? _commandHandler;
        private static ConfigManager? _configManager;

        private static bool _ready = false;

        public static void Main()
        {
            _discord = new(Client);
            
            _commandHandler = new(Client);
            
            _discord.OnNewReactionRole += NewReactionRole;
            Client.Log += Log;
            Client.Ready += Client_Ready;
            while (!_ready) ;
            while (_commandHandler.Running) ;
        }

        private static void NewReactionRole(object? sender, NewReactionRole e)
        {
            ReactionRole reactionRole = new()
            {
                Role = e.Role,
                Message = e.Message,
                Reaction = e.Emote
            };
            _configManager!.ReactionRoles.ReactionRoleList.Add(reactionRole);
            _configManager.Save();
        }


        #region Custom Events

        private static void TikTokLiveStarted(object? sender, bool isLive) => _ = _discord.TikTokLiveStarted(isLive);

        #endregion

        #region Discord Net Events

        private static Task Client_Ready()
        {
            _tikTokHandler = new();
            _configManager = new(Client);
            _commandHandler!.ConfigManager = _configManager;
            _tikTokHandler.LiveStarted += TikTokLiveStarted;
            _ready = true;
            return Task.CompletedTask;
        }

        private static Task Log(LogMessage arg)
        {
            _commandHandler!.WriteConsoleLine(arg.Message);
            return Task.CompletedTask;
        }

        #endregion
    }
}