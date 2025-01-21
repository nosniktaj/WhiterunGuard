using System.Net.Sockets;
using System.Xml.Linq;
using Discord.WebSocket;

namespace WhiterunConfig
{
    public class Guild : BaseXML
    {
        public readonly ReactionRoles ReactionRoles;

        public SocketGuild SocketGuild { get; }

        public ulong GuildId => SocketGuild.Id;

        public Guild(SocketGuild socketSocketGuild)
        {
            SocketGuild = socketSocketGuild;
            ReactionRoles = new ReactionRoles(SocketGuild);
        }

        public override void Load(XElement? xElement)
        {
            if (xElement != null) ReactionRoles.Load(xElement.Element("ReactionRoles"));
        }

        public override XElement GenerateXml()
        {
            var xElement = new XElement("Guild");
            xElement.SetAttributeValue("ID", SocketGuild.Id);
            xElement.Add(ReactionRoles.GenerateXml());
            return xElement;
        }
    }
}