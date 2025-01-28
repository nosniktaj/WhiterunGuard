using System.Runtime.InteropServices;
using System.Xml.Linq;
using Discord.WebSocket;

namespace WhiterunConfig
{
    public class ConfigManager : BaseXML
    {
        private static readonly string _configDirectory = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? Path.Combine(Path.GetPathRoot(Environment.SystemDirectory)!, "Nosniktaj")
            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library", "Application Support", "Nosniktaj")
                : Path.Combine("/etc", "opt", "nosniktaj");


        private static readonly string _configFilePath = Path.Combine(_configDirectory, "config.xml");

        public readonly Guilds Guilds;

        private readonly DiscordSocketClient _client;


        public ConfigManager(DiscordSocketClient client)
        {
            _client = client;
            Guilds = new Guilds(_client);
        }


        public void Load()
        {
            if (!File.Exists(_configFilePath)) Save();
            var xElement = XElement.Load(_configFilePath);

            Guilds.Load(xElement.Element("Guilds"));
            while (_client.Guilds.Count < 1) ;
            Console.WriteLine("Loaded Config");
            Save();
        }

        public void Save()
        {
            Directory.CreateDirectory(_configDirectory);
            var xElement = GenerateXml();
            xElement.Save(_configFilePath);
        }

        public override XElement GenerateXml()
        {
            var retval = new XElement("Configuration");
            var guildElement = new XElement("Guilds");
            retval.Add(Guilds.GenerateXml());
            return retval;
        }
    }
}