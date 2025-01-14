using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace WhiterunConfig
{
    public class ConfigManager
    {
        private static readonly string _configDirectory = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(Path.GetPathRoot(Environment.SystemDirectory), "Nosniktaj")
            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? @"/Library/Frameworks/Python.framework/Versions/3.10/Python"
                : Path.Combine("etc", "opt", "nosniktaj");


        private static readonly string _configFilePath = Path.Combine(_configDirectory, "config.xml");

        public ReactionRoles ReactionRoles = new();


        public ConfigManager()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            if (!File.Exists(_configFilePath)) Save();
            var xElement = XElement.Load(_configFilePath);
            ReactionRoles.Load(xElement.Element("ReactionRoles"));
        }

        public void Save()
        {
            Directory.CreateDirectory(_configDirectory);
            var xElement = GenerateXML();
            xElement.Save(_configFilePath);
        }

        private XElement GenerateXML()
        {
            var retval = new XElement("Configration");
            retval.Add(ReactionRoles.GenerateXML());
            return retval;
        }
    }
}