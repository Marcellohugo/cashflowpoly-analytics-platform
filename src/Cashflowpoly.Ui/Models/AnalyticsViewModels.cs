namespace Cashflowpoly.Ui.Models;

public sealed class AnalyticsSearchViewModel
{
    public string SessionId { get; set; } = string.Empty;
    public AnalyticsSessionResponseDto? Result { get; set; }
    public string? ErrorMessage { get; set; }
}
