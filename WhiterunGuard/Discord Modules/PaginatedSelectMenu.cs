using Discord;
using Discord.WebSocket;

public class PaginatedSelectMenu
{
    
    public EventHandler<string> onSelect;
    private readonly DiscordSocketClient _client;
    private readonly string _customIdBase;
    private readonly List<SelectMenuOptionBuilder> _options;
    private readonly int _pageSize;
    private int _currentPage;

    public PaginatedSelectMenu(DiscordSocketClient client, string customIdBase, List<SelectMenuOptionBuilder> options, int pageSize = 25)
    {
        _client = client;
        _customIdBase = customIdBase;
        _options = options;
        _pageSize = pageSize;
        _currentPage = 0;

        _client.ButtonExecuted += HandleButtonInteraction;
        _client.SelectMenuExecuted += HandleSelectMenuInteraction;
    }

    private int TotalPages => (int)Math.Ceiling(_options.Count / (double)_pageSize);

    public ComponentBuilder BuildPage()
    {
        var menu = new SelectMenuBuilder()
            .WithCustomId($"{_customIdBase}_menu")
            .WithPlaceholder("Choose an option...")
            .WithOptions(GetCurrentPageOptions());
        
        return new ComponentBuilder()
            .WithSelectMenu(menu)
            .WithButton("Previous", $"{_customIdBase}_prev", disabled: _currentPage == 0)
            .WithButton("Next", $"{_customIdBase}_next", disabled: _currentPage >= TotalPages - 1);
    }

    private List<SelectMenuOptionBuilder> GetCurrentPageOptions()
    {
        return _options
            .Skip(_currentPage * _pageSize)
            .Take(_pageSize)
            .ToList();
    }

    private async Task HandleButtonInteraction(SocketMessageComponent component)
    {
        if (!component.Data.CustomId.StartsWith(_customIdBase))
            return;

        if (component.Data.CustomId.EndsWith("_prev"))
            _currentPage = Math.Max(0, _currentPage - 1);
        else if (component.Data.CustomId.EndsWith("_next"))
            _currentPage = Math.Min(TotalPages - 1, _currentPage + 1);

        await component.UpdateAsync(msg =>
        {
            msg.Components = BuildPage().Build();
        });
    }

    private async Task HandleSelectMenuInteraction(SocketMessageComponent component)
    {
        if (!component.Data.CustomId.StartsWith(_customIdBase))
            return;

        var selectedValue = component.Data.Values.FirstOrDefault();
        if (!string.IsNullOrEmpty(selectedValue))
        {
            onSelect?.Invoke(this, selectedValue);
        }
    }
}