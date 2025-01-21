using System.Xml.Linq;
using Discord.WebSocket;

namespace WhiterunConfig
{
    public class ReactionRoles(SocketGuild guild) : BaseXML
    {
        public readonly List<ReactionRole> ReactionRoleList = [];

        public override void Load(XElement xElement)
        {
            ReactionRoleList.Clear();
            foreach (var (element, input) in xElement.Elements("ReactionRole")
                         .Select(reactionRole => (reactionRole, new ReactionRole(guild))))
            {
                input.Load(element);
                if (input.Message != null)
                    ReactionRoleList.Add(input);
            }
        }

        public override XElement GenerateXml()
        {
            var xElement = new XElement("ReactionRoles");
            foreach (var reactionRole in ReactionRoleList) xElement.Add(reactionRole.GenerateXml());
            return xElement;
        }
    }
}