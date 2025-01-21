using System.Xml.Linq;
using Discord.WebSocket;

namespace WhiterunConfig
{
    public class Guilds : BaseXML
    {
        public readonly List<Guild?> GuildList = [];
        private readonly DiscordSocketClient _client;

        public Guilds(DiscordSocketClient client) => _client = client;

        public override void Load(XElement? xElement)
        {
            foreach (var socketGuild in _client.Guilds)
            {
                var guild = new Guild(socketGuild);
                if (xElement != null && xElement.Elements("Guild")
                        .Any(gld => gld.Attribute("ID")?.Value == socketGuild.Id.ToString()))
                    guild.Load(xElement?.Elements("Guild")
                        .First(gld => gld.Attribute("ID")!.Value == socketGuild.Id.ToString()));
                GuildList.Add(guild);
            }
        }

        public override XElement GenerateXml()
        {
            var xElement = new XElement("Guilds");
            foreach (var guild in GuildList) xElement.Add(guild!.GenerateXml());
            return xElement;
        }

        public Guild? GetGuild(ulong? id)
        {
            if (GuildList.Any(guild => guild != null && guild.GuildId == id))
            {
                return GuildList.First(guild => guild!.GuildId == id);
            }
            else if (_client.Guilds.Any(guild => guild.Id == id))
            {
                var guild = new Guild(_client.Guilds.First(guild => guild.Id == id));
                GuildList.Add(guild);
                return guild;
            }

            return null;
        }
    }
}