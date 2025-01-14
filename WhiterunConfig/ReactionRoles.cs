using System.Xml.Linq;

namespace WhiterunConfig
{
    public class ReactionRoles
    {
        private readonly List<ReactionRole> _reactionRoleList = [];

        public void Load(XElement xElement)
        {
            _reactionRoleList.Clear();
            foreach (var (element, input) in xElement.Elements("ReactionRole")
                         .Select(reactionRole => (reactionRole, new ReactionRole())))
            {
                input.Load(element);
                _reactionRoleList.Add(input);
            }
        }

        public XElement GenerateXml()
        {
            var xElement = new XElement("ReactionRoles");
            foreach (var reactionRole in _reactionRoleList) xElement.Add(reactionRole.GenerateXml());
            return xElement;
        }
    }
}