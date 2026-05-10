using System.Collections.Frozen;

namespace Cashflowpoly.Ui.Infrastructure;

internal static partial class UiTextLexicon
{
    internal static FrozenDictionary<string, (string Id, string En)> Build()
    {
        var terms = new Dictionary<string, (string Id, string En)>(StringComparer.OrdinalIgnoreCase);
        AddCore(terms);
        AddSessions(terms);
        AddPlayers(terms);
        AddRulesets(terms);
        AddRulebook(terms);
        return terms.ToFrozenDictionary(StringComparer.OrdinalIgnoreCase);
    }

    private static partial void AddCore(Dictionary<string, (string Id, string En)> terms);

    private static partial void AddSessions(Dictionary<string, (string Id, string En)> terms);

    private static partial void AddPlayers(Dictionary<string, (string Id, string En)> terms);

    private static partial void AddRulesets(Dictionary<string, (string Id, string En)> terms);

    private static partial void AddRulebook(Dictionary<string, (string Id, string En)> terms);
}