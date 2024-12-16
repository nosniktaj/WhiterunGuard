using Discord.WebSocket;

namespace WhiterunGuard
{
    public static class StaticExtensions
    {
        public static DateTime TimeAccurateToMinutes(this DateTime dt) =>
            new(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);

        public static bool IsAdmin(this SocketGuildUser? user) => user is not null &&
                                                                  (user.GuildPermissions.Administrator ||
                                                                   user.Id == 6174712406673981542);
    }
}