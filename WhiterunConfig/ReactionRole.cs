using System.Xml.Linq;
using Discord;
using Discord.WebSocket;

namespace WhiterunConfig
{
    public class ReactionRole
    {
        public IEmote? Reaction { get; set; }
        public IMessage? Message { get; set; }
        public IRole? Role { get; set; }

        public void Load(XElement xElement, SocketGuild guild)
        {
            if (xElement == null || guild == null)
                throw new ArgumentNullException($"{nameof(xElement)} or {nameof(guild)} cannot be null.");

            var channelId = xElement.GetUlong("Channel", 0);
            if (channelId == 0)
                return;

            var channel = guild.GetTextChannel(channelId);
            if (channel == null)
                return;

            var messageId = xElement.GetUlong("Message", 0);
            if (messageId == 0)
                return;

            IMessage? message = null;
            try
            {
                message = channel.GetMessageAsync(messageId).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to retrieve message: {ex.Message}");
                return;
            }

            if (message == null)
                return;

            Message = message;
            Role = guild.GetRole(xElement.GetUlong("Role", 0));
            Reaction = xElement.GetEmote("Reaction", null);
        }


        public XElement GenerateXml()
        {
            var xElement = new XElement("ReactionRole");
            xElement.Add(new XElement("Channel", Message!.Channel.Id));
            xElement.Add(new XElement("Message", Message.Id));
            xElement.Add(new XElement("Role", Role!.Id));
            xElement.Add(new XElement("Reaction", Reaction));
            return xElement;
        }
    }
}