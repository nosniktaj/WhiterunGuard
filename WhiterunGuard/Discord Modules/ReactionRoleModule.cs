using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using WhiterunConfig;

namespace WhiterunGuard
{
    [Group("reactionrole", "Reaction Role Commands")]
    public class ReactionRoleModule : InteractionModuleBase<SocketInteractionContext>
    {
        #region Private Fields

        private static ConfigManager _configManager;
        private static DiscordSocketClient _client;

        #endregion

        #region Constructor

        public ReactionRoleModule(ConfigManager configManager, DiscordSocketClient client)
        {
            _configManager = configManager;
            _client = client;
        }

        #endregion


        #region Message

        [Group("message", "Reaction Role Message Commands")]
        public class ReactionRoleMessageModule : InteractionModuleBase<SocketInteractionContext>
        {
            #region New

            [SlashCommand("create", "Add a new reaction role message")]
            public async Task NewReactionRoleCommand()
            {
                var mb = new ModalBuilder()
                    .WithTitle("Create new reaction role message")
                    .WithCustomId("new_reaction_role")
                    .AddTextInput("Title", "title", placeholder: "What does this role give you? e.g \"Age\"")
                    .AddTextInput("Unique Identifier", "uid",
                        placeholder: "A clear unique value. e.g \"age_reaction\" ")
                    .AddTextInput("Message", "message", TextInputStyle.Paragraph,
                        "List the emojis and corresponding roles");

                await RespondWithModalAsync(mb.Build());
            }

            [ModalInteraction("new_reaction_role", true)]
            public async Task NewReactionRoleModal(ReactionMessageModal modal)
            {
                if (Context.Interaction.HasResponded)
                    return;

                var title = modal.TitleInput;
                var caption = modal.MessageInput;
                var uid = modal.UidInput;
                var guild = _configManager.Guilds.GetGuild(Context.Guild.Id);
                if (guild == null ||
                    guild.ReactionRoles.ReactionRoleList.Any(r => r.Uid == uid))
                {
                    await Context.Interaction.RespondAsync(
                        "A reaction message with this UID already exists. Use /editreactionmessage to edit it or /deletereactionmessage to delete it. ");
                    return;
                }

                var embed = new EmbedBuilder()
                    .WithTitle(title)
                    .WithDescription(caption)
                    .WithColor(new Color(66, 66, 66));
                var message = await Context.Channel.SendMessageAsync(embed: embed.Build());
                ReactionRole reactionRole = new(guild.SocketGuild)
                {
                    Uid = uid,
                    Message = message
                };
                await Context.Interaction.RespondAsync("Message Created", ephemeral: true);
                guild!.ReactionRoles.ReactionRoleList.Add(reactionRole);
                _configManager.Save();
            }

            #endregion

            #region Edit

            [SlashCommand("edit", "Edit a reaction role message")]
            public async Task EditRoleMessage()
            {
                var menuBuilder = new SelectMenuBuilder()
                    .WithPlaceholder("Select a message")
                    .WithCustomId("edit_reaction_role_message")
                    .WithMinValues(1)
                    .WithMaxValues(1);
                foreach (var reactionRole in _configManager.Guilds.GetGuild(Context.Guild.Id)!.ReactionRoles
                             .ReactionRoleList.Where(reactionRole =>
                                 !string.IsNullOrEmpty(reactionRole.Uid) && reactionRole.Message!.Embeds.Count == 1))
                    menuBuilder.AddOption(reactionRole.Message!.Embeds.First().Title, reactionRole.Uid);

                var builder = new ComponentBuilder()
                    .WithSelectMenu(menuBuilder);
                await RespondAsync("What Message would you like to edit?", components: builder.Build(),
                    ephemeral: true);
            }

            [ComponentInteraction("edit_reaction_role_message", true)]
            public async Task EditRoleMessageSelect(string[] uidSelection)
            {
                if (Context.Interaction.HasResponded)
                    return;

                var uid = uidSelection[0];
                var guild = _configManager.Guilds.GetGuild(Context.Guild.Id);
                var reactionRole = guild!.ReactionRoles.ReactionRoleList.First(r => r.Uid == uid);
                var currentTitle = reactionRole.Message?.Embeds.First().Title;
                var currentDescription = reactionRole.Message?.Embeds.First().Description;
                var mb = new ModalBuilder()
                    .WithTitle($"Editing Reaction Role Message {uid}")
                    .WithCustomId($"edit_reaction_role_message:{uid}")
                    .AddTextInput("Title", "title", placeholder: "What does this role give you? e.g \"Age\"",
                        value: currentTitle)
                    .AddTextInput("Message", "message", TextInputStyle.Paragraph,
                        "List the emojis and corresponding roles", value: currentDescription);

                await Context.Interaction.RespondWithModalAsync(mb.Build());
            }

            [ModalInteraction("edit_reaction_role_message:*", true)]
            public async Task EditRoleMessageModal(string id, ReactionMessageModal modal)
            {
                if (Context.Interaction.HasResponded)
                    return;

                var guild = _configManager.Guilds.GetGuild(Context.Guild.Id);
                if (guild!.ReactionRoles.ReactionRoleList.Any(r => r.Uid == id))
                {
                    var title = modal.TitleInput;
                    var caption = modal.MessageInput;

                    if (guild!.ReactionRoles.ReactionRoleList.First(r => r.Uid == id).Message is IUserMessage message)
                    {
                        var embed = new EmbedBuilder()
                            .WithTitle(title)
                            .WithDescription(caption)
                            .WithColor(new Color(66, 66, 66))
                            .Build();

                        _configManager.Save();
                        await message.ModifyAsync(msg => msg.Embeds = new[] { embed });
                        await Context.Interaction.RespondAsync("Message Updated", ephemeral: true);
                    }
                    else
                    {
                        await Context.Interaction.RespondAsync(
                            "Message not found, if you believe this is an issue please contact dev@nosniktaj.com");
                    }
                }
            }

            #endregion

            #region Remove

            [SlashCommand("delete", "Remove a reaction role message")]
            private async Task RemoveRoleMessage()
            {
                var menuBuilder = new SelectMenuBuilder()
                    .WithPlaceholder("Select a message")
                    .WithCustomId("delete_reaction_role_message")
                    .WithMinValues(1)
                    .WithMaxValues(1);
                foreach (var reactionRole in _configManager.Guilds.GetGuild(Context.Guild.Id)!.ReactionRoles
                             .ReactionRoleList.Where(reactionRole =>
                                 !string.IsNullOrEmpty(reactionRole.Uid) && reactionRole.Message!.Embeds.Count == 1))
                    menuBuilder.AddOption(reactionRole.Message!.Embeds.First().Title, reactionRole.Uid);

                var builder = new ComponentBuilder()
                    .WithSelectMenu(menuBuilder);
                await RespondAsync("What Message would you like to delete?", components: builder.Build(),
                    ephemeral: true);
            }

            [ComponentInteraction("delete_reaction_role_message", true)]
            public async Task DeletetRoleMessageSelect(string[] uidSelection)
            {
                if (Context.Interaction.HasResponded)
                    return;

                var uid = uidSelection[0];
                var guild = _configManager.Guilds.GetGuild(Context.Guild.Id);
                var reactionRole = guild!.ReactionRoles.ReactionRoleList.First(r => r.Uid == uid);
                if (reactionRole.Message is IUserMessage message) await message.DeleteAsync();
                guild.ReactionRoles.ReactionRoleList.Remove(reactionRole);


                await RespondAsync("Message deleted", ephemeral: true);
            }

            #endregion
        }

        #endregion

        #region Reactions

        [Group("reaction", "Reaction Role Message Commands")]
        public class ReactionRoleReactionModule : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("add", "Add a reaction role to an existing message")]
            public async Task AddReactionRoleCommand(string messageId, IRole role, string emoji)
            {
                await RespondAsync($"Added role {role.Name} with emoji {emoji} to message {messageId}");
            }

            [SlashCommand("remove", "Remove a reaction role from an existing message")]
            public async Task RemoveReactionCommand()
            {
            }
        }

        #endregion
    }
}