using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using WhiterunConfig;

namespace WhiterunGuard
{
    public class ReactionRoleModule(ConfigManager configManager) : InteractionModuleBase<SocketInteractionContext>
    {
        [SlashCommand("addrolemessage", "Add a new reaction role message")]
        public async Task NewReactionRoleCommand()
        {
            var mb = new ModalBuilder()
                .WithTitle("Create new reaction role message")
                .WithCustomId("newreactionrole")
                .AddTextInput("Title", "title", placeholder: "What does this role give you? e.g \"Age\"")
                .AddTextInput("Unique Identifier", "uid", placeholder: "A clear unique value. e.g \"age_reaction\" ")
                .AddTextInput("Message", "message", TextInputStyle.Paragraph,
                    "List the emojis and corresponding roles");

            await RespondWithModalAsync(mb.Build());
        }

        [ModalInteraction("newreactionrole")]
        public async Task NewReactionRoleModal(SocketModal modal)
        {
            var title = modal.Data.Components.First(c => c.CustomId == "title").Value;
            var caption = modal.Data.Components.First(c => c.CustomId == "message").Value;
            var embed = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(caption)
                .WithColor(new Color(66, 66, 66));
            var message = await modal.Channel.SendMessageAsync(embed: embed.Build());

            /*var create = new ReactionRole
            {
                UID = modal.Data.Components.First(c => c.CustomId == "uid").Value,
                Message = message
            };

            configManager.ReactionRoles.ReactionRoleList.Add(create);*/
        }

        [SlashCommand("addrolereaction", "Add a reaction role to an existing message")]
        public async Task AddReactionRoleCommand(string messageId, IRole role, string emoji)
        {
            // Implement logic for adding a reaction role here
            await RespondAsync($"Added role {role.Name} with emoji {emoji} to message {messageId}");
        }
    }
}