using System.Xml.Linq;
using Discord;

namespace WhiterunConfig
{
    public static class XmlUtilities
    {
        public static string GetString(this XElement xElement, XName xName, string defaultValue) =>
            (xElement.Element(xName) is not null
             && !string.IsNullOrWhiteSpace(xElement.Element(xName)!.Value)
                ? xElement.Element(xName)?.Value
                : defaultValue)!;

        public static IEmote GetEmote(this XElement xElement, XName xName, Emoji defaultValue) =>
            xElement.Element(xName) is not null
                ? Emoji.TryParse(xElement.Element(xName)!.Value, out var emoji) ? emoji :
                Emote.TryParse(xElement.Element(xName)!.Value, out var emote) ? emote :
                defaultValue
                : defaultValue;
    }
}