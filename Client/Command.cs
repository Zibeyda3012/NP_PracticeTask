namespace Client;

public class Command
{
    public int ProcessId { get; set; }

    public string? ProcessName { get; set; }

    public string? Type { get; set; }

    public string? Content { get; set; } = null;
}
