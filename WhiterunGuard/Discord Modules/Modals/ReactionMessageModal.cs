using Discord;
using Discord.Interactions;

namespace WhiterunGuard
{
    public class ReactionMessageModal : IModal
    {
        public string Title => "Create new reaction role message";

        [InputLabel("Title")]
        [ModalTextInput("title", placeholder: "What does this role give you? e.g \"Age\"")]
        public required string TitleInput { get; set; }

        [InputLabel("Unique Identifier")]
        [ModalTextInput("uid", placeholder: "A clear unique value. e.g \"age_reaction\"")]
        public string? UidInput { get; set; }

        [InputLabel("Message")]
        [ModalTextInput("message", TextInputStyle.Paragraph, "List the emojis and corresponding roles")]
        public required string MessageInput { get; set; }
    }
}