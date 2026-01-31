namespace Cashflowpoly.Ui.Models;

public sealed class RulesetListViewModel
{
    public List<RulesetListItemDto> Items { get; init; } = new();
    public string? ErrorMessage { get; init; }
}

public sealed class CreateRulesetViewModel
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string ConfigJson { get; set; } = "{}";
    public string? ErrorMessage { get; set; }
}
