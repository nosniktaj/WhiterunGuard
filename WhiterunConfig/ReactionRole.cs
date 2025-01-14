using System.Xml.Linq;
using Discord;

namespace WhiterunConfig
{
    public class ReactionRole
    {
        public IEmote Reaction;
        public string Message { get; set; }
        public string Role { get; set; }


        public void Load(XElement xElement)
        {
            Message = xElement.GetString("Message", string.Empty);
            Reaction = xElement.GetEmote("Reaction", new Emoji("\U0001f495"));
            Role = xElement.GetString("Role", string.Empty);
        }

        public XElement GenerateXML()
        {
            var xElement = new XElement("ReactionRole");
            xElement.Add(new XElement("Message", Message));
            xElement.Add(new XElement("Reaction", Reaction));
            xElement.Add(new XElement("Role", Role));
            return xElement;
        }
    }
}