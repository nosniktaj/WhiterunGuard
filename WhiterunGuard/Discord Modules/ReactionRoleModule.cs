using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using WhiterunConfig;

namespace WhiterunGuard
{
    public class ReactionRoleModule : InteractionModuleBase<SocketInteractionContext>
    {
        #region Private Fields

        private readonly ConfigManager _configManager;
        private readonly DiscordSocketClient _client;

        #endregion

        #region Constructor

        public ReactionRoleModule(ConfigManager configManager, DiscordSocketClient client)
        {
            _configManager = configManager;
            _client = client;
            _client.ModalSubmitted += ModalSubmitted;
            _client.SelectMenuExecuted += SelectMenuExecuted;
        }

        #endregion

        #region Interaction Handlers

        private async Task ModalSubmitted(SocketModal modal)
        {
            switch (modal.Data.CustomId)
            {
                case "new_reaction_role":
                    await NewReactionRoleModal(modal);
                    break;
                case var id when id.StartsWith("edit_reaction_role_message:"):
                    await EditRoleMessageModal(modal);
                    break;
            }
        }

        private async Task SelectMenuExecuted(SocketMessageComponent selectMenu)
        {
            switch (selectMenu.Data.CustomId)
            {
                case "edit_reaction_role_message":
                    await EditRoleMessageSelect(selectMenu);
                    break;
            }
        }

        #endregion

        #region Message

        #region New

        [SlashCommand("addrolemessage", "Add a new reaction role message")]
        public async Task NewReactionRoleCommand()
        {
            var mb = new ModalBuilder()
                .WithTitle("Create new reaction role message")
                .WithCustomId("new_reaction_role")
                .AddTextInput("Title", "title", placeholder: "What does this role give you? e.g \"Age\"")
                .AddTextInput("Unique Identifier", "uid", placeholder: "A clear unique value. e.g \"age_reaction\" ")
                .AddTextInput("Message", "message", TextInputStyle.Paragraph,
                    "List the emojis and corresponding roles");

            await RespondWithModalAsync(mb.Build());
        }

        public async Task NewReactionRoleModal(SocketModal modal)
        {
            if (modal.HasResponded)
                return;

            var title = modal.Data.Components.First(c => c.CustomId == "title").Value;
            var caption = modal.Data.Components.First(c => c.CustomId == "message").Value;
            var uid = modal.Data.Components.First(c => c.CustomId == "uid").Value;
            if (_configManager.ReactionRoles.ReactionRoleList.Count != 0 && _configManager.ReactionRoles.ReactionRoleList.Any(r => r.UID == uid))
            {
                await modal.RespondAsync(
                    "A reaction message with this UID already exists. Use /editreactionmessage to edit it or /deletereactionmessage to delete it. ");
                return;
            }

            var embed = new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(caption)
                .WithColor(new Color(66, 66, 66));
            var message = await modal.Channel.SendMessageAsync(embed: embed.Build());
            ReactionRole reactionRole = new()
            {
                UID = modal.Data.Components.First(c => c.CustomId == "uid").Value,
                Message = message
            };
            await modal.RespondAsync("Message Created", ephemeral: true);
            _configManager.ReactionRoles.ReactionRoleList.Add(reactionRole);
            _configManager.Save();
        }

        #endregion

        #region Edit

        [SlashCommand("editrolemessage", "Edit a reaction role message")]
        public async Task EditRoleMessage()
        {
            var menuBuilder = new SelectMenuBuilder()
                .WithPlaceholder("Select a message")
                .WithCustomId("edit_reaction_role_message")
                .WithMinValues(1);
            foreach (var reactionRole in _configManager.ReactionRoles.ReactionRoleList.Where(reactionRole =>
                         !string.IsNullOrEmpty(reactionRole.UID) && reactionRole.Message!.Embeds.Count == 1))
                menuBuilder.AddOption(reactionRole.Message!.Embeds.First().Title, reactionRole.UID);

            var builder = new ComponentBuilder()
                .WithSelectMenu(menuBuilder);
            await RespondAsync("What Message would you like to edit?", components: builder.Build(), ephemeral: true);
        }

        public async Task EditRoleMessageSelect(SocketMessageComponent selectMenu)
        {
            if (selectMenu.HasResponded)
                return;

            var uid = selectMenu.Data.Values.First();
            var reactionRole = _configManager.ReactionRoles.ReactionRoleList.First(r => r.UID == uid);
            var currentTitle = reactionRole.Message.Embeds.First().Title;
            var currentDescription = reactionRole.Message.Embeds.First().Description;
            var mb = new ModalBuilder()
                .WithTitle($"Editing Reaction Role Message {uid}")
                .WithCustomId($"edit_reaction_role_message:{uid}")
                .AddTextInput("Title", "title", placeholder: "What does this role give you? e.g \"Age\"",
                    value: currentTitle)
                .AddTextInput("Message", "message", TextInputStyle.Paragraph,
                    "List the emojis and corresponding roles", value: currentDescription);

            await selectMenu.RespondWithModalAsync(mb.Build());
        }

        public async Task EditRoleMessageModal(SocketModal modal)
        {
            if (modal.HasResponded)
                return;

            var uid = modal.Data.CustomId.Split(':')[1];
            if (_configManager.ReactionRoles.ReactionRoleList.Any(r => r.UID == uid))
            {
                var title = modal.Data.Components.First(c => c.CustomId == "title").Value;
                var caption = modal.Data.Components.First(c => c.CustomId == "message").Value;
                var message =
                    _configManager.ReactionRoles.ReactionRoleList.First(r => r.UID == uid).Message as
                        IUserMessage ;

                var embed = new EmbedBuilder()
                    .WithTitle(title)
                    .WithDescription(caption)
                    .WithColor(new Color(66, 66, 66))
                    .Build();


               await message.ModifyAsync(msg => msg.Embeds = new[]{embed});
                await modal.RespondAsync("Message Updated", ephemeral: true);
            }
        }

        #endregion
        
        #region Remove

        [SlashCommand("removerolemessage", "Remove a reaction role message")]
        private async Task RemoveRoleMessage()
        {
            
        }
        
        #endregion

        #endregion

        #region Reactions

        [SlashCommand("addrolereaction", "Add a reaction role to an existing message")]
        public async Task AddReactionRoleCommand(string messageId, IRole role, string emoji)
        {
            await RespondAsync($"Added role {role.Name} with emoji {emoji} to message {messageId}");
        }

        #endregion
    }
}