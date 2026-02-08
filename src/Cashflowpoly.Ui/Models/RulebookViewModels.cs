namespace Cashflowpoly.Ui.Models;

public sealed class RulebookPageViewModel
{
    public string Title { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public List<RulebookSectionViewModel> Sections { get; init; } = new();
    public List<RulebookScoreItemViewModel> Scoring { get; init; } = new();
}

public sealed class RulebookSectionViewModel
{
    public string Heading { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public List<string> Points { get; init; } = new();
}

public sealed class RulebookScoreItemViewModel
{
    public string Category { get; init; } = string.Empty;
    public string Rule { get; init; } = string.Empty;
}
