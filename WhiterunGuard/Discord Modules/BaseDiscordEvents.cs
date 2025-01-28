using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using WhiterunConfig;

namespace WhiterunGuard;

public class BaseDiscordEvents : InteractionModuleBase<SocketInteractionContext>
{
    private static ConfigManager _configManager;
    private static DiscordSocketClient _client;
    
    public BaseDiscordEvents(ConfigManager configManager, DiscordSocketClient client)
    {
        _configManager = configManager;
        _client = client;

        _client.JoinedGuild += OnJoinedGuild;
    }
    private static Task OnJoinedGuild(SocketGuild arg)
    {
        _configManager.Guilds.GuildList.Add(new Guild(arg));
        _configManager.Save();
        return Task.CompletedTask;
    }
}