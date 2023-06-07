namespace OpenFlier.DevUtils;
public class PluginFileModel
{
    public string? FilePath
    {
        get; set;
    }
    public string? FileName
    {
        get; set;
    }
    public bool IsPluginMain
    {
        get; set;
    } = false;
}
