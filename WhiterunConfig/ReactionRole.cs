using System.Xml.Linq;
using Discord;
using Discord.WebSocket;

namespace WhiterunConfig
{
    public class ReactionRole(SocketGuild guild) : BaseXML
    {
        public string? Uid { get; set; } = string.Empty;
        public IMessage? Message { get; set; }

        public readonly List<(IRole role, IEmote emote)> Reactions = [];

        public override void Load(XElement xElement)
        {
            Uid = xElement.GetString("UID", string.Empty);
            var channel = guild.GetTextChannel(xElement.GetUlong("Channel", 0));
            if (channel == null)
                return;

            IMessage? message;

            try
            {
                message = channel.GetMessageAsync(xElement.GetUlong("Message", 0)).GetAwaiter().GetResult();
            }
            catch
            {
                return;
            }

            if (message == null)
                return;

            Message = message;

            if (xElement.Element("Reactions") == null) return;
            Reactions.AddRange(GetReactions(xElement.Element("Reactions")!));
        }


        public override XElement GenerateXml()
        {
            var xElement = new XElement("ReactionRole");
            xElement.Add(new XElement("UID", Uid));
            xElement.Add(new XElement("Channel", Message!.Channel.Id));
            xElement.Add(new XElement("Message", Message.Id));
            var reactions = new XElement("Reactions");
            foreach (var reaction in Reactions)
            {
                reactions.Add(new XElement("Role", reaction.role.Id));
                reactions.Add(new XElement("Emote", reaction.emote));
            }

            xElement.Add(reactions);

            return xElement;
        }

        private List<(IRole role, IEmote emote)> GetReactions(XElement xElement)
        {
            List<(IRole Role, IEmote emote)> reactions = [];
            foreach (var reaction in xElement.Elements())
            {
                var role = guild.GetRole(reaction.GetUlong("Role", 0));
                var emote = xElement.GetEmote("Emote", null);
                if (role is null || emote is null) break;
                reactions.Add((role, emote));
            }

            return reactions;
        }
    }
}