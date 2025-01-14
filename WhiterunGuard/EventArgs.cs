using Discord;

namespace WhiterunGuard
{
    public class NewReactionRole(IEmote emote, IMessage message, IRole role) : EventArgs
    {
        public IEmote Emote = emote;
        public IMessage Message = message;
        public IRole Role = role;
    }
}