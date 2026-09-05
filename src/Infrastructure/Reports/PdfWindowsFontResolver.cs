using PdfSharp.Fonts;

namespace NerjaLogisticsERP.Infrastructure.Reports;

/// <summary>
/// PDFsharp 6 targets net10.0 (not net10.0-windows), so it no longer has automatic access to
/// GDI/system fonts and needs an explicit <see cref="IFontResolver"/>. This one reads TTF files
/// straight out of the Windows Fonts folder — simplest option given the app is Windows-hosted.
/// If this ever moves to a Linux/container host, swap this for a resolver over embedded font
/// files instead.
/// </summary>
public class PdfWindowsFontResolver : IFontResolver
{
    public byte[] GetFont(string faceName)
    {
        var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), faceName);
        return File.ReadAllBytes(path);
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        var fileName = (isBold, isItalic) switch
        {
            (true, true) => "arialbi.ttf",
            (true, false) => "arialbd.ttf",
            (false, true) => "ariali.ttf",
            _ => "arial.ttf",
        };
        return new FontResolverInfo(fileName);
    }
}
