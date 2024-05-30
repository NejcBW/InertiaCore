namespace InertiaCore.Models;

/// <summary>
/// A class representing the Inertia's Page object.
/// </summary>
internal class Page
{
    public Dictionary<string, object?> Props { get; set; } = default!;
    public string Component { get; set; } = default!;
    public string? Version { get; set; }
    public string Url { get; set; } = default!;
}