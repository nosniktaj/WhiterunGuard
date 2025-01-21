using System.Xml.Linq;

namespace WhiterunConfig
{
    public class BaseXML
    {
        public virtual void Load(XElement xElement)
        {
        }

        public virtual XElement GenerateXml() => new("Base");
    }
}