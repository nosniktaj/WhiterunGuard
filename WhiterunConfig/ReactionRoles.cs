using System.Xml.Linq;

namespace WhiterunConfig
{
    public class ReactionRoles
    {
        private readonly List<ReactionRole> ReactionRoleList = new();

        public void Load(XElement xElement)
        {
            ReactionRoleList.Clear();
            foreach (var (element, input) in xElement.Elements("ReactionRole")
                         .Select(reactionRole => (reactionRole, new ReactionRole())))
            {
                input.Load(element);
                ReactionRoleList.Add(input);
            }
        }

        public XElement GenerateXML()
        {
            var xElement = new XElement("ReactionRoles");
            foreach (var ReactionRole in ReactionRoleList) xElement.Add(ReactionRole.GenerateXML());
            return xElement;
        }
    }
}