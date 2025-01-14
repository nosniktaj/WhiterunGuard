using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace WhiterunConfig
{
    public class ConfigManager
    {
        private static readonly string _configDirectory = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(Path.GetPathRoot(Environment.SystemDirectory)!, "Nosniktaj")
            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? @"/Library/Frameworks/Python.framework/Versions/3.10/Python"
                : Path.Combine("/etc", "opt", "nosniktaj");


        private static readonly string _configFilePath = Path.Combine(_configDirectory, "config.xml");

        public readonly ReactionRoles ReactionRoles = new();


        public ConfigManager()
        {
            LoadConfig();
        }

        private void LoadConfig()
        {
            if (!File.Exists(_configFilePath)) Save();
            var xElement = XElement.Load(_configFilePath);
            ReactionRoles.Load(xElement.Element("ReactionRoles")!);
        }

        public void Save()
        {
            Directory.CreateDirectory(_configDirectory);
            var xElement = GenerateXml();
            xElement.Save(_configFilePath);
        }

        private XElement GenerateXml()
        {
            var retval = new XElement("Configuration");
            retval.Add(ReactionRoles.GenerateXml());
            return retval;
        }
    }
}