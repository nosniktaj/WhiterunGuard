using System.Xml.Linq;
using Discord.WebSocket;

namespace WhiterunConfig
{
    public class ReactionRoles
    {
        public readonly List<ReactionRole> ReactionRoleList = [];

        public void Load(XElement xElement, SocketGuild guild)
        {
            ReactionRoleList.Clear();
            foreach (var (element, input) in xElement.Elements("ReactionRole")
                         .Select(reactionRole => (reactionRole, new ReactionRole())))
            {
                input.Load(element, guild);
                if (!(input.Message == null || input.Role == null || input.Reaction == null))
                    ReactionRoleList.Add(input);
            }
        }

        public XElement GenerateXml()
        {
            var xElement = new XElement("ReactionRoles");
            foreach (var reactionRole in ReactionRoleList) xElement.Add(reactionRole.GenerateXml());
            return xElement;
        }
    }
}