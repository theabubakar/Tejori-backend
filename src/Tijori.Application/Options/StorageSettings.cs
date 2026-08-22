namespace Tijori.Application.Options;

public class StorageSettings
{
    public const string SectionName = "Storage";

    public long DefaultTotalBytes { get; set; } = 2_147_483_648;
    public string UploadRootPath { get; set; } = "uploads";
    public long MaxUploadBytes { get; set; } = 10_485_760;
}
