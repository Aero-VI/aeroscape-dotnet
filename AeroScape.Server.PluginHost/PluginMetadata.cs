namespace AeroScape.Server.PluginHost;

/// <summary>
/// Plugin metadata loaded from plugin.yml file.
/// </summary>
public class PluginMetadata
{
    public string Name { get; set; } = "";
    public string Main { get; set; } = ""; // Fully qualified class name
    public string Version { get; set; } = "";
    public string Author { get; set; } = "";
    public string Description { get; set; } = "";
    public List<string> Dependencies { get; set; } = new();
    public string ApiVersion { get; set; } = "1.0";
}