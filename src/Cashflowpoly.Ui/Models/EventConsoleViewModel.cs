namespace Cashflowpoly.Ui.Models;

public sealed class EventConsoleViewModel
{
    public string EventJson { get; set; } = string.Empty;
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ResponseJson { get; set; }

    public static string BuildDefaultSample()
    {
        return """
{
  "event_id": "00000000-0000-0000-0000-000000000000",
  "session_id": "00000000-0000-0000-0000-000000000000",
  "player_id": "00000000-0000-0000-0000-000000000000",
  "actor_type": "PLAYER",
  "timestamp": "2026-02-03T12:00:00Z",
  "day_index": 1,
  "weekday": "MON",
  "turn_number": 1,
  "sequence_number": 1,
  "action_type": "order.passed",
  "ruleset_version_id": "00000000-0000-0000-0000-000000000000",
  "payload": {
    "order_card_id": "ORD-002",
    "required_ingredient_card_ids": ["ING-002"],
    "income": 12
  }
}
""";
    }
}
