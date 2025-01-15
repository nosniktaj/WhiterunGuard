using System.Runtime.InteropServices;
using System.Xml.Linq;
using Discord.WebSocket;

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

        private readonly DiscordSocketClient _client;
        private ulong _guildId;


        public ConfigManager(DiscordSocketClient client)
        {
            _client = client;
            LoadConfig();
        }


        public ulong GuildId
        {
            get => _guildId;
            set
            {
                _guildId = value;
                Save();
            }
        }

        private void LoadConfig()
        {
            if (!File.Exists(_configFilePath)) Save();
            var xElement = XElement.Load(_configFilePath);
            _guildId = xElement.GetUlong("GuildId", 1205836076187394079);
            ReactionRoles.Load(xElement.Element("ReactionRoles")!, _client.GetGuild(_guildId));
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
            retval.Add(new XElement("GuildId", _guildId));
            retval.Add(ReactionRoles.GenerateXml());
            return retval;
        }
    }
}