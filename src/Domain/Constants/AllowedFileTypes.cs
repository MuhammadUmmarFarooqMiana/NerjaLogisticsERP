namespace NerjaLogisticsERP.Domain.Constants;

public static class AllowedFileTypes
{
    // contentType -> (max size in bytes, allowed extensions)
    public static readonly Dictionary<string, (long MaxSizeBytes, string[] Extensions)> Map =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["application/pdf"] = (10 * 1024 * 1024, new[] { ".pdf" }),
            ["image/jpeg"] = (5 * 1024 * 1024, new[] { ".jpg", ".jpeg" }),
            ["image/png"] = (5 * 1024 * 1024, new[] { ".png" }),
            ["application/msword"] = (10 * 1024 * 1024, new[] { ".doc" }),
            ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"] = (10 * 1024 * 1024, new[] { ".docx" }),
            ["application/vnd.ms-excel"] = (10 * 1024 * 1024, new[] { ".xls" }),
            ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"] = (10 * 1024 * 1024, new[] { ".xlsx" }),
        };

    public static bool IsAllowed(string contentType, string fileName, long sizeBytes)
    {
        if (string.IsNullOrWhiteSpace(contentType) || string.IsNullOrWhiteSpace(fileName))
            return false;

        // Strip any "; charset=..." style parameters some clients append.
        var normalizedContentType = contentType.Split(';')[0].Trim();

        if (!Map.TryGetValue(normalizedContentType, out var rule))
            return false;

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return rule.Extensions.Contains(ext) && sizeBytes > 0 && sizeBytes <= rule.MaxSizeBytes;
    }
}
